using backend.Models;

public class ChannelInvite
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public int UserId { get; set; } // Người được mời
    public int InvitedById { get; set; } // Người gửi lời mời
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Accepted { get; set; } = false;
    public bool Rejected { get; set; } = false;

    public ChatChannel Channel { get; set; }
    public User User { get; set; }
    public User InvitedBy { get; set; }
}
