// backend/Models/ChatChannel.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ChatChannel : Event
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsPrivate { get; set; } = false;

    public bool RequireInvite { get; set; } = true;

    public bool VoiceEnabled { get; set; } = true;

    public int MaxMembers { get; set; } = 100;

    public bool ModeratorToolsEnabled { get; set; } = true;

    public string? PasswordHash { get; set; } // Mật khẩu (nếu có)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public VoiceChannel VoiceChannel { get; set; }

    // Navigation
    public ICollection<ChannelMember> Members { get; set; } = new List<ChannelMember>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}