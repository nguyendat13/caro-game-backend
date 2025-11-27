namespace backend.DTOs.ChatChannel
{
    public class ChannelMemberDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsModerator { get; set; }
        public bool IsMuted { get; set; }
        public DateTime? MutedUntil { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
