// backend/Models/MessageReaction.cs (Bonus – hỗ trợ react tin nhắn)
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class MessageReaction
{
    public int MessageId { get; set; }
    public ChatMessage Message { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [StringLength(10)]
    public string Emoji { get; set; } = string.Empty; // Ví dụ: "Like", "Haha", "Heart"

   
}