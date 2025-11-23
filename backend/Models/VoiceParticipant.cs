// backend/Models/VoiceParticipant.cs
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class VoiceParticipant
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int VoiceChannelId { get; set; }
    public VoiceChannel VoiceChannel { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }

    public bool IsMuted { get; set; } = false;
    public bool IsDeafened { get; set; } = false;
    public bool IsSpeaking { get; set; } = false; // Dùng để hiện viền xanh khi đang nói


}