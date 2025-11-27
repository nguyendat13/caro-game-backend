namespace backend.DTOs.ChatChannel
{
    public class UserChannelSummaryDTO
    {
        public int TotalChannels { get; set; }
        public List<ChatChannelResponseDTO> Channels { get; set; } = new();
    }
}
