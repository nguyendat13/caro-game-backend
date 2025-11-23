// backend/Models/VoiceChannel.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class VoiceChannel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Liên kết với ChatChannel (mỗi kênh chat có thể có 1 voice channel)
    public int ChatChannelId { get; set; }
    public ChatChannel ChatChannel { get; set; } = null!;

    public int MaxUsers { get; set; } = 50;

    public bool IsTemporary { get; set; } = false; // Kênh tạm (tự xóa khi trống)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    // Navigation
    public ICollection<VoiceParticipant> Participants { get; set; } = new List<VoiceParticipant>();
}