namespace backend.DTOs.Game
{
    public class GameStatsDTO
    {
        public string GameName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinRate { get; set; }
    }
}
