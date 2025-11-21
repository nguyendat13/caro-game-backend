namespace backend.DTOs.Game
{
    public class RecentGameDTO
    {
        public int GameId { get; set; }
        public string Opponent { get; set; }   // Tên đối thủ
        public string Result { get; set; }     // "Win", "Lose", "Draw"
        public DateTime PlayedAt { get; set; }
        public string Type { get; set; }       // Loại game (Caro, Chess, ...)
    }
}
