using backend.DTOs.Game;

namespace backend.DTOs.User
{
    public class UserStatsDTO
    {
        public int TotalGamesPlayed { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public double TotalWinRate { get; set; }
        public List<GameStatsDTO> GamesByType { get; set; } = new List<GameStatsDTO>();
    }
}
