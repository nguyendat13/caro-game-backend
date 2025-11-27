namespace backend.DTOs.ChatMessage
{
    public class ChatMessageCreateDTO
    {
        public int ChannelId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
