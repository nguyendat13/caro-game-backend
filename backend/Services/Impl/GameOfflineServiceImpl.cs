using backend.DTOs.Game;
using backend.Models;
using Microsoft.EntityFrameworkCore;

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

            if (game == null || game.Status != GameStatus.Ongoing)
                throw new Exception("Game not found or already finished.");

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

                return new GameMoveDTO
                {
                    PlayerMove = new MoveDetailDTO(moveDto.X, moveDto.Y, "Player"),
                    BotMove = null
                };
            }

            // 🤖 Nước đi của bot
            var botMove = await GenerateBotMoveAsync(moveDto.GameId, nextOrder + 1);

            if (botMove != null)
            {
                _context.GameMoves.Add(botMove);
                await _context.SaveChangesAsync();

                // 🧠 Kiểm tra thắng của bot (đảm bảo null-safe)
                if (botMove?.PlayerId != null && CheckWinCondition(moveDto.GameId, botMove.PlayerId.Value))
                {
                    game.WinnerId = botMove.PlayerId;
                    game.Status = GameStatus.Finished;
                    await _context.SaveChangesAsync();
                }

                return new GameMoveDTO
                {
                    PlayerMove = new MoveDetailDTO(moveDto.X, moveDto.Y, "Player"),
                    BotMove = new MoveDetailDTO(botMove.X, botMove.Y, "Bot")
                };
            }

            // 🚫 Nếu bot không thể đánh (full bàn)
            return new GameMoveDTO
            {
                PlayerMove = new MoveDetailDTO(moveDto.X, moveDto.Y, "Player"),
                BotMove = null
            };
        }


        private async Task<GameMove?> GenerateBotMoveAsync(int gameId, int moveOrder)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null) return null;

            var rand = new Random();
            var existingMoves = game.Moves.Select(m => (m.X, m.Y)).ToHashSet();

            // Giả định bàn 15x15
            for (int i = 0; i < 100; i++)
            {
                byte x = (byte)rand.Next(0, 15);
                byte y = (byte)rand.Next(0, 15);

                if (!existingMoves.Contains((x, y)))
                {
                    return new GameMove
                    {
                        GameId = gameId,
                        PlayerId = null, // ✅ bot user trong DB
                        X = x,
                        Y = y,
                        MoveOrder = moveOrder,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }

            return null; // full bàn
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
            if (playerId == null)
                return false; // bot hoặc nước đi không hợp lệ

            // TODO: Thêm logic kiểm tra thắng
            return false;
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
