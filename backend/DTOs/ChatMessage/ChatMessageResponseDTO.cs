using backend.Models;
namespace backend.DTOs.ChatMessage
{
    public class ChatMessageResponseDTO
    {
        public int MessageId { get; set; }
        public int ChannelId { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;

        public List<MessageReactionDTO> Reactions { get; set; } = new();

        public ChatMessageResponseDTO() { }

        public ChatMessageResponseDTO(backend.Models.ChatMessage m)
        {
            MessageId = m.Id;
            ChannelId = m.ChannelId ?? 0;
            Content = m.Content;
            CreatedAt = m.CreatedAt;

            SenderId = m.SenderId;
            SenderUsername = m.Sender.Username;

            Reactions = m.Reactions.Select(r => new MessageReactionDTO
            {
                UserId = r.UserId,
                Emoji = r.Emoji
            }).ToList();
        }
    }
}
