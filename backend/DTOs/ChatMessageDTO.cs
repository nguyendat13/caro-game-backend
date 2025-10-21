namespace backend.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
