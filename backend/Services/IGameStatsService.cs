using backend.DTOs.Game;

namespace backend.Services
{
    public interface IGameStatsService
    {
        Task<List<RecentGameDTO>> GetRecentGamesAsync(int userId, int count = 5);
        Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(string gameType, int top = 10);
    }
}
