namespace backend.DTOs.ChatChannel
{
    public class ChatChannelUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsPrivate { get; set; }
        public bool? RequireInvite { get; set; }
        public bool? VoiceEnabled { get; set; }
        public int? MaxMembers { get; set; }
        public string? Password { get; set; }
        public int? EventId { get; set; }

    }
}
