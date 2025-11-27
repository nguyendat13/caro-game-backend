namespace backend.DTOs.ChatChannel
{
    public class ChatChannelCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsPrivate { get; set; } = false;
        public bool RequireInvite { get; set; } = true;
        public bool VoiceEnabled { get; set; } = true;

        public int MaxMembers { get; set; } = 100;

        public string? Password { get; set; } // Optional
        public int? EventId { get; set; } 

    }
}
