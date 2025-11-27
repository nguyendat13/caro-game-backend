namespace backend.DTOs.ChatChannel
{
    public class ChannelInviteDTO
    {
        public int Id { get; set; }

        // Thông tin kênh
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = null!;
        public string? ChannelDescription { get; set; }

        // Thông tin người được mời
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        // Thông tin người mời
        public int InvitedById { get; set; }
        public string InvitedByName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public bool Accepted { get; set; }
        public bool Rejected { get; set; }
    }
}
