// backend/Models/ChatMessage.cs (ĐÃ S�A ĐỂ HỖ TRỢ KÊNH CHAT)
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ChatMessage
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Người gửi
    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    // Có thể thuộc về Game HOẶC Channel
    public int? GameId { get; set; }
    public Game? Game { get; set; }

    public int? ChannelId { get; set; }
    public ChatChannel? Channel { get; set; }

    // Navigation
    public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
}