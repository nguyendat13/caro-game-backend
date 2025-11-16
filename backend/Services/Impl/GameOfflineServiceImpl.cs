using backend.DTOs.Game;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.Services
{
    public class GameOfflineServiceImpl : IGameOfflineService
    {
        private readonly CaroDbContext _context;
        private readonly Random _random = new Random();

        public GameOfflineServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameDTO>> GetAllGamesAsync()
        {
            var games = await _context.Games
                .Include(g => g.Moves)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return games.Select(g => new GameDTO
            {
                GameId = g.GameId,
                PlayerXId = g.PlayerXId,
                Status = g.Status.ToString(),
                WinnerId = g.WinnerId,
                CreatedAt = g.CreatedAt,
                Moves = g.Moves.Select(m => new MoveDetailDTO(m.X, m.Y, m.PlayerId == null ? "Bot" : "Player")).ToList()
            }).ToList();
        }

        public async Task<GameDTO> CreateOfflineGameAsync(int playerId)
        {
            var user = await _context.Users.FindAsync(playerId);
            if (user == null)
                throw new KeyNotFoundException($"UserId {playerId} không tồn tại.");

            var game = new Game
            {
                PlayerXId = playerId,
                PlayerOId = null,
                Status = GameStatus.Ongoing,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return new GameDTO
            {
                GameId = game.GameId,
                PlayerXId = game.PlayerXId,
                Status = game.Status.ToString(),
                WinnerId = game.WinnerId,
                CreatedAt = game.CreatedAt,
                Moves = new List<MoveDetailDTO>()
            };
        }

        public async Task<GameMoveDTO> MakeMoveAsync(GameMoveCreateDTO moveDto)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == moveDto.GameId);

            if (game == null)
                return GameMoveDTO.Fail("Không tìm thấy ván cờ.");

            if (game.Status != GameStatus.Ongoing)
                return GameMoveDTO.Fail("Ván cờ đã kết thúc, không thể đi thêm.");

            // Không cho đánh trùng
            if (game.Moves.Any(m => m.X == moveDto.X && m.Y == moveDto.Y))
                return GameMoveDTO.Fail("Ô này đã được đánh rồi, vui lòng chọn ô khác.");

            int nextOrder = game.Moves.Count + 1;

            // Player Move
            var playerMove = new GameMove
            {
                GameId = moveDto.GameId,
                PlayerId = moveDto.PlayerId,
                X = moveDto.X,
                Y = moveDto.Y,
                MoveOrder = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.GameMoves.Add(playerMove);
            await _context.SaveChangesAsync();

            // Kiểm tra thắng Player
            if (moveDto.PlayerId != null && CheckWinCondition(moveDto.GameId, moveDto.PlayerId.Value))
            {
                game.WinnerId = moveDto.PlayerId;
                game.Status = GameStatus.Finished;
                await _context.SaveChangesAsync();

                return GameMoveDTO.SuccessPlayerWin(moveDto.X, moveDto.Y);
            }

            // BOT move
            var botMove = await GenerateBotMoveAsync(moveDto.GameId, nextOrder + 1);

            if (botMove != null)
            {
                _context.GameMoves.Add(botMove);
                await _context.SaveChangesAsync();

                // Kiểm tra thắng Bot
                if (CheckWinCondition(moveDto.GameId, botMove.PlayerId))
                {
                    game.WinnerId = botMove.PlayerId;
                    game.Status = GameStatus.Finished;
                    await _context.SaveChangesAsync();

                    return GameMoveDTO.SuccessBotWin(
                        moveDto.X, moveDto.Y,
                        botMove.X, botMove.Y
                    );
                }

                return GameMoveDTO.SuccessMove(
                    moveDto.X, moveDto.Y,
                    botMove.X, botMove.Y
                );
            }

            // Không còn chỗ trống -> hòa
            game.Status = GameStatus.Finished;
            await _context.SaveChangesAsync();

            return GameMoveDTO.Draw(moveDto.X, moveDto.Y);
        }

        private async Task<GameMove?> GenerateBotMoveAsync(int gameId, int moveOrder)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null) return null;

            int size = 15;
            var moves = game.Moves.ToList();

            var playerMoves = moves
                .Where(m => m.PlayerId != null)
                .Select(m => (m.X, m.Y))
                .ToList();

            // ✅ 1. BOT CHẶN NẾU NGƯỜI CHƠI SẮP THẮNG
            foreach (var (px, py) in playerMoves)
            {
                foreach (var (dx, dy) in new (int, int)[]
                {
                    (1, 0),   // ngang
                    (0, 1),   // dọc
                    (1, 1),   // chéo xuống
                    (1, -1)   // chéo lên
                })
                {
                    int count = 1;
                    int x = px + dx, y = py + dy;

                    // ✅ FIX: Thêm bounds check vào while loop
                    while (x >= 0 && y >= 0 && x < size && y < size && playerMoves.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x += dx;
                        y += dy;
                    }

                    if (count >= 4)
                    {
                        if (x >= 0 && y >= 0 && x < size && y < size && !moves.Any(m => m.X == x && m.Y == y))
                        {
                            return new GameMove
                            {
                                GameId = gameId,
                                PlayerId = null,
                                X = (byte)x,
                                Y = (byte)y,
                                MoveOrder = moveOrder,
                                CreatedAt = DateTime.UtcNow
                            };
                        }

                        int backX = px - dx, backY = py - dy;
                        if (backX >= 0 && backY >= 0 && backX < size && backY < size && !moves.Any(m => m.X == backX && m.Y == backY))
                        {
                            return new GameMove
                            {
                                GameId = gameId,
                                PlayerId = null,
                                X = (byte)backX,
                                Y = (byte)backY,
                                MoveOrder = moveOrder,
                                CreatedAt = DateTime.UtcNow
                            };
                        }
                    }
                }
            }

            // ✅ 2. FIX: Kiểm tra playerMoves rỗng trước
            if (!playerMoves.Any())
            {
                return new GameMove
                {
                    GameId = gameId,
                    PlayerId = null,
                    X = 7,
                    Y = 7,
                    MoveOrder = moveOrder,
                    CreatedAt = DateTime.UtcNow
                };
            }

            // ✅ 3. ĐÁNH GẦN NGƯỜI CHƠI NHẤT
            var lastPlayerMove = playerMoves.Last();
            var nearbyCells = new List<(int X, int Y)>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int newX = lastPlayerMove.X + dx;
                    int newY = lastPlayerMove.Y + dy;

                    if (newX >= 0 && newY >= 0 && newX < size && newY < size &&
                        !moves.Any(m => m.X == newX && m.Y == newY))
                    {
                        nearbyCells.Add((newX, newY));
                    }
                }
            }

            if (nearbyCells.Any())
            {
                var chosen = nearbyCells[_random.Next(nearbyCells.Count)];
                return new GameMove
                {
                    GameId = gameId,
                    PlayerId = null,
                    X = (byte)chosen.X,
                    Y = (byte)chosen.Y,
                    MoveOrder = moveOrder,
                    CreatedAt = DateTime.UtcNow
                };
            }

            return null;
        }

        public async Task<GameDTO?> GetGameAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null) return null;

            return new GameDTO
            {
                GameId = game.GameId,
                PlayerXId = game.PlayerXId,
                Status = game.Status.ToString(),
                WinnerId = game.WinnerId,
                CreatedAt = game.CreatedAt,
                // ✅ FIX: Đổi == 0 thành == null
                Moves = game.Moves.Select(m => new MoveDetailDTO(m.X, m.Y, m.PlayerId == null ? "Bot" : "Player")).ToList()
            };
        }

        private bool CheckWinCondition(int gameId, int? playerId)
        {
            if (playerId == null) return false;

            var moves = _context.GameMoves
                .Where(m => m.GameId == gameId && m.PlayerId == playerId)
                .Select(m => new { m.X, m.Y })
                .ToList();

            if (moves.Count < 5) return false;

            var moveSet = moves.Select(m => (m.X, m.Y)).ToHashSet();

            int[][] directions = new int[][]
            {
                new int[]{1, 0},
                new int[]{0, 1},
                new int[]{1, 1},
                new int[]{1, -1}
            };

            foreach (var move in moves)
            {
                foreach (var dir in directions)
                {
                    int count = 1;
                    int dx = dir[0], dy = dir[1];

                    int x = move.X + dx, y = move.Y + dy;
                    while (moveSet.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x += dx;
                        y += dy;
                    }

                    x = move.X - dx;
                    y = move.Y - dy;
                    while (moveSet.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x -= dx;
                        y -= dy;
                    }

                    if (count >= 5)
                        return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null)
                return false;

            _context.GameMoves.RemoveRange(game.Moves);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}