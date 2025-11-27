using backend.DTOs.ChatMessage;

namespace backend.Services
{
    public interface IChatMessageService
    {
        Task<ChatMessageResponseDTO> SendMessageAsync(ChatMessageCreateDTO dto, int senderId);
        Task<List<ChatMessageResponseDTO>> GetMessagesByChannelAsync(int channelId);
        Task<bool> ReactToMessageAsync(int messageId, int userId, string emoji);
    }
}
