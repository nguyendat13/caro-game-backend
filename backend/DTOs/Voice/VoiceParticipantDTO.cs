using backend.Models;

namespace backend.DTOs.Voice
{
    public class VoiceParticipantDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public bool IsMuted { get; set; }
        public bool IsDeafened { get; set; }
        public bool IsSpeaking { get; set; }

        public DateTime JoinedAt { get; set; }

        public VoiceParticipantDTO() { }

        public VoiceParticipantDTO(VoiceParticipant p)
        {
            UserId = p.UserId;
            Username = p.User.Username;
            JoinedAt = p.JoinedAt;
            IsMuted = p.IsMuted;
            IsDeafened = p.IsDeafened;
            IsSpeaking = p.IsSpeaking;
        }
    }
}
