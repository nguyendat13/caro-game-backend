using backend.Models;
    namespace backend.DTOs.ChatChannel
{
    public class ChatChannelResponseDTO
    {
        public int ChannelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsPrivate { get; set; }
        public bool RequireInvite { get; set; }
        public bool VoiceEnabled { get; set; }
        public int MaxMembers { get; set; }

        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public int MemberCount { get; set; }

        public List<ChannelMemberDTO> Members { get; set; } = new();

        public ChatChannelResponseDTO() { }

        public ChatChannelResponseDTO(backend.Models.ChatChannel c)
        {
            ChannelId = c.Id;
            Name = c.Name;
            Description = c.Description;
            IsPrivate = c.IsPrivate;
            RequireInvite = c.RequireInvite;
            VoiceEnabled = c.VoiceEnabled;
            MaxMembers = c.MaxMembers;
            CreatorId = c.CreatorId;
            CreatedAt = c.CreatedAt;
            MemberCount = c.Members.Count;

            Members = c.Members.Select(m => new ChannelMemberDTO
            {
                UserId = m.UserId,
                Username = m.User?.Username ?? "Unknown",
                JoinedAt = m.JoinedAt,
                IsModerator = m.IsModerator,
                IsMuted = m.IsMuted,
                MutedUntil = m.MutedUntil
            }).ToList();

        }
    }
}
