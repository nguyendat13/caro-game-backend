namespace backend.Models
{
    public class Game
    {
        public int GameId { get; set; }           // PK
        public int? PlayerXId { get; set; }       // FK User
        public int? PlayerOId { get; set; }       // FK User
        public int? WinnerId { get; set; }        // FK User, nullable
        public GameStatus Status { get; set; } = GameStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User PlayerX { get; set; }
        public User PlayerO { get; set; }
        public User Winner { get; set; }
        public ICollection<GameMove> Moves { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
