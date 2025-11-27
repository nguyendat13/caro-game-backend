using backend.DTOs.ChatChannel;
using backend.Models;
using static backend.Services.Impl.ChatChannelServiceImpl;

namespace backend.Services
{
    public interface IChatChannelService
    {
        //tạo kênh chat
        Task<ChatChannelResponseDTO> CreateChannelAsync(ChatChannelCreateDTO dto, int userId);

        //kiểm tra channel trong csdl
        Task<ChatChannel?> GetChannelForJoinAsync(int channelId);
        
        //tham gia kênh chat
        Task<ServiceResult<bool>> JoinChannelAsync(int channelId, int userId, string? password);

        //rời nhóm
        Task<bool> LeaveChannelAsync(int channelId, int userId);

        //cập nhật nhóm
        Task<bool> UpdateChannelAsync(int channelId, ChatChannelUpdateDTO dto, int currentUserId);

        //xóa nhóm
        Task<bool> DeleteChannelAsync(int channelId, int currentUserId);

        //xem kênh cộng đồng 
        Task<List<ChatChannelResponseDTO>> GetPublicChannelsAsync();

        //xem kênh riêng
        Task<ChatChannelResponseDTO?> GetChannelByIdAsync(int channelId, int? userId = null);

        //xem yêu cầu tham gia nhóm
        Task<List<ChannelInviteDTO>> GetInvitesForUserAsync(int userId);

        //chấp nhận invites
        Task<ServiceResult<bool>> AcceptInviteAsync(int inviteId, int userId);

        //từ chối invites
        Task<ServiceResult<bool>> RejectInviteAsync(int inviteId, int userId);

        //xem nhóm của user
        Task<UserChannelSummaryDTO> GetUserChannelsAsync(int userId);
    }
}
