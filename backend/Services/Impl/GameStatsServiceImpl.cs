using backend.DTOs.Game;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class GameStatsServiceImpl : IGameStatsService
    {
        private readonly CaroDbContext _context;

        public GameStatsServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecentGameDTO>> GetRecentGamesAsync(int userId, int count = 5)
        {
            var recentGames = await _context.Games
                .Where(g => g.PlayerXId == userId || g.PlayerOId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Take(count)
                .Include(g => g.PlayerX)
                .Include(g => g.PlayerO)
                .ToListAsync();

            var result = recentGames.Select(g => new RecentGameDTO
            {
                GameId = g.GameId,
                Opponent = g.PlayerXId == userId ? g.PlayerO?.Username ?? "Bot" : g.PlayerX.Username,
                PlayedAt = g.CreatedAt,
                Type = g.Type.ToString(),
                Result = g.WinnerId == null ? "Hòa" :
                         g.WinnerId == userId ? "Thắng" : "Thua"
            }).ToList();

            return result;
        }

        public async Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(string gameType, int top = 100)
        {
            // Chỉ lấy các trận ĐÃ HOÀN THÀNH của loại game yêu cầu
            var finishedGames = _context.Games
                .AsNoTracking()
                .Where(g => g.Type.ToString() == gameType && g.Status == GameStatus.Finished);

            var leaderboard = await finishedGames
                .GroupBy(g => g.PlayerXId ?? g.PlayerOId!) // Người chơi (X hoặc O)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalGames = g.Count(),
                    Wins = g.Count(x => x.WinnerId == g.Key),
                    Draws = g.Count(x => x.WinnerId == null),
                    Losses = g.Count(x => x.WinnerId != null && x.WinnerId != g.Key)
                })
                .Where(x => x.TotalGames > 0)
                // Xếp hạng chuẩn: Thắng nhiều trước → Win rate cao → Nhiều trận
                .OrderByDescending(x => x.Wins)
                .ThenByDescending(x => x.TotalGames > 0 ? (double)x.Wins / x.TotalGames : 0)
                .ThenByDescending(x => x.TotalGames)
                .Take(top)
                // Join với bảng Users để lấy thông tin Username, FullName
                .Join(_context.Users,
                    stats => stats.UserId,
                    user => user.UserId,
                    (stats, user) => new LeaderboardEntryDTO
                    {
                        Rank = 0, // sẽ gán sau
                        UserId = user.UserId.ToString(),
                        Username = user.Username,
                        FullName = user.FullName,
                        TotalGames = stats.TotalGames,
                        Wins = stats.Wins,
                        Losses = stats.Losses,
                        Draws = stats.Draws,
                        WinRate = stats.TotalGames > 0
                            ? Math.Round((double)stats.Wins / stats.TotalGames * 100, 1)
                            : 0
                    })
                .ToListAsync();

            // Gán Rank thứ tự cuối cùng (vì EF không hỗ trợ ROW_NUMBER() dễ dàng trong GroupBy)
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
            }

            return leaderboard;
        }

    }
}
