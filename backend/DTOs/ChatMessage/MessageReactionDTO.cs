namespace backend.DTOs.ChatMessage
{
    public class MessageReactionDTO
    {
        public int UserId { get; set; }
        public string Emoji { get; set; } = string.Empty;
    }
}
