namespace backend.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }   // <-- primary key

        public int GameId { get; set; }           // FK Game
        public int SenderId { get; set; }         // FK User
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Game Game { get; set; }
        public User Sender { get; set; }
    }
}
