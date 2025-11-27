using backend.Models;

namespace backend.DTOs.Voice
{
    public class VoiceChannelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int ChatChannelId { get; set; }
        public int MaxUsers { get; set; }
        public bool IsTemporary { get; set; }

        public List<VoiceParticipantDTO> Participants { get; set; } = new();

        public VoiceChannelDTO() { }

        public VoiceChannelDTO(VoiceChannel v)
        {
            Id = v.Id;
            Name = v.Name;
            ChatChannelId = v.ChatChannelId;
            MaxUsers = v.MaxUsers;
            IsTemporary = v.IsTemporary;

            Participants = v.Participants.Select(p => new VoiceParticipantDTO(p)).ToList();
        }
    }
}
