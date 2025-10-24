namespace backend.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
        public DateTime? DisconnectedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
