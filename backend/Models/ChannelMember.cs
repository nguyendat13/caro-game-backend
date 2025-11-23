// backend/Models/ChannelMember.cs
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class ChannelMember
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ChannelId { get; set; }
    public ChatChannel Channel { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public bool IsModerator { get; set; } = false;
    public bool IsMuted { get; set; } = false;
    public DateTime? MutedUntil { get; set; }

   
}