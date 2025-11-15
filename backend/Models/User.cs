namespace backend.Models
{
    public class User
    {
        public int UserId { get; set; }           // PK
        public string Username { get; set; }      // unique
        public string FullName { get; set; }
        public string Email { get; set; }         // unique
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Role Role { get; set; }
        public ICollection<Game> GamesAsX { get; set; }
        public ICollection<Game> GamesAsO { get; set; }
        public ICollection<GameMove> Moves { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();

    }
}
