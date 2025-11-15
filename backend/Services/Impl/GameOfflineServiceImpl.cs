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
            // Kiểm tra user có tồn tại không
            var user = await _context.Users.FindAsync(playerId);
            if (user == null)
                throw new KeyNotFoundException($"UserId {playerId} không tồn tại.");

            // Tạo mới game offline
            var game = new Game
            {
                PlayerXId = playerId,
                PlayerOId = null, // Bot
                Status = GameStatus.Ongoing,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Lưu vào DB
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Trả về DTO (chỉ trả thông tin cần thiết)
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
                throw new Exception("Không tìm thấy ván cờ.");

            if (game.Status != GameStatus.Ongoing)
                throw new Exception("Ván cờ đã kết thúc, không thể đi thêm.");

            // 🧩 Kiểm tra trùng vị trí
            if (game.Moves.Any(m => m.X == moveDto.X && m.Y == moveDto.Y))
                throw new Exception("Ô này đã được đánh rồi, vui lòng chọn ô khác.");

            // 🧩 Người chơi đi
            int nextOrder = game.Moves.Count + 1;

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

            // 🏁 Kiểm tra thắng của người chơi
            if (moveDto.PlayerId != null && CheckWinCondition(moveDto.GameId, moveDto.PlayerId.Value))
            {
                game.WinnerId = moveDto.PlayerId;
                game.Status = GameStatus.Finished;
                await _context.SaveChangesAsync();

                throw new Exception("🎉 Bạn đã chiến thắng ván cờ!");
            }

            // 🤖 Nước đi của bot
            var botMove = await GenerateBotMoveAsync(moveDto.GameId, nextOrder + 1);

            if (botMove != null)
            {
                _context.GameMoves.Add(botMove);
                await _context.SaveChangesAsync();

                // 🧠 Kiểm tra thắng của bot
                if (CheckWinCondition(moveDto.GameId, botMove.PlayerId))
                {
                    game.WinnerId = botMove.PlayerId;
                    game.Status = GameStatus.Finished;
                    await _context.SaveChangesAsync();

                    throw new Exception("🤖 Bot đã chiến thắng!");
                }

                return new GameMoveDTO
                {
                    PlayerMove = new MoveDetailDTO(moveDto.X, moveDto.Y, "Player"),
                    BotMove = new MoveDetailDTO(botMove.X, botMove.Y, "Bot")
                };
            }

            // 🚫 Nếu bot không thể đánh (full bàn)
            game.Status = GameStatus.Finished;
            await _context.SaveChangesAsync();
            throw new Exception("Ván cờ kết thúc, không còn ô trống.");
        }


        private async Task<GameMove?> GenerateBotMoveAsync(int gameId, int moveOrder)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null) return null;

            int size = 15; // bàn cờ 15x15
            var moves = game.Moves.ToList();

            // Lấy toàn bộ tọa độ người chơi
            var playerMoves = moves
                .Where(m => m.PlayerId != null) // người chơi thật
                .Select(m => (m.X, m.Y))
                .ToList();

            // ✅ 1. BOT CHẶN NẾU NGƯỜI CHƠI SẮP THẮNG
            foreach (var (px, py) in playerMoves)
            {
                foreach (var (dx, dy) in new (int, int)[]
                {
            (1, 0),  // ngang
            (0, 1),  // dọc
            (1, 1),  // chéo xuống
            (1, -1)  // chéo lên
                })
                {
                    int count = 1;

                    // Đếm liên tiếp của người chơi theo hướng (dx, dy)
                    int x = px + dx, y = py + dy;
                    while (playerMoves.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x += dx; y += dy;
                    }

                    // Nếu có 4 quân liên tiếp, chặn ô kế tiếp
                    if (count >= 4)
                    {
                        // Ô phía trước
                        if (x >= 0 && y >= 0 && x < size && y < size && !moves.Any(m => m.X == x && m.Y == y))
                        {
                            return new GameMove
                            {
                                GameId = gameId,
                                PlayerId = null, // Bot
                                X = (byte)x,
                                Y = (byte)y,
                                MoveOrder = moveOrder,
                                CreatedAt = DateTime.UtcNow
                            };
                        }

                        // Ô phía sau
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

            // ✅ 2. KHÔNG NGUY HIỂM → ĐÁNH GẦN NGƯỜI CHƠI NHẤT
            var lastPlayerMove = playerMoves.LastOrDefault();
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
                var chosen = nearbyCells[new Random().Next(nearbyCells.Count)];
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

            // ✅ 3. Nếu full bàn → không thể đi
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
                Moves = game.Moves.Select(m => new MoveDetailDTO(m.X, m.Y, m.PlayerId == 0 ? "Bot" : "Player")).ToList()
            };
        }

        private bool CheckWinCondition(int gameId, int? playerId)
        {
            if (playerId == null) return false;

// Lấy tất cả nước đi của người chơi trong game
var moves = _context.GameMoves
    .Where(m => m.GameId == gameId && m.PlayerId == playerId)
    .Select(m => new { m.X, m.Y })
    .ToList();

            if (moves.Count < 5) return false; // chưa đủ để thắng

            // Chuyển sang HashSet để tra cứu nhanh
            var moveSet = moves.Select(m => (m.X, m.Y)).ToHashSet();

            // 4 hướng cần kiểm tra (ngang, dọc, chéo chính, chéo phụ)
            int[][] directions = new int[][]
            {
    new int[]{1, 0},   // ngang
    new int[]{0, 1},   // dọc
    new int[]{1, 1},   // chéo chính
    new int[]{1, -1}   // chéo phụ
            };

            foreach (var move in moves)
            {
                foreach (var dir in directions)
                {
                    int count = 1;

                    // kiểm tra 1 phía
                    int dx = dir[0], dy = dir[1];
                    int x = move.X + dx, y = move.Y + dy;
                    while (moveSet.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x += dx;
                        y += dy;
                    }

                    // kiểm tra phía ngược lại
                    x = move.X - dx;
                    y = move.Y - dy;
                    while (moveSet.Contains(((byte)x, (byte)y)))
                    {
                        count++;
                        x -= dx;
                        y -= dy;
                    }

                    if (count >= 5)
                        return true; // thắng
                }
            }

            return false; // chưa thắng

}

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.Moves) // include để xóa luôn moves
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null)
                return false;

            // Xóa tất cả các moves trước
            _context.GameMoves.RemoveRange(game.Moves);

            // Xóa luôn game
            _context.Games.Remove(game);

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
