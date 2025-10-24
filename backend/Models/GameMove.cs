namespace backend.Models
{
    public class GameMove
    {
        public int Id { get; set; }
        public int GameId { get; set; }           // FK Game
        public int? PlayerId { get; set; }         // FK User
        public byte X { get; set; }               // 0-based
        public byte Y { get; set; }               // 0-based
        public int MoveOrder { get; set; }        // Thứ tự nước đi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Game Game { get; set; }
        public User Player { get; set; }
    }
}
