using backend.DTOs.ChatChannel;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Impl;

public class ChatChannelServiceImpl : IChatChannelService
{
    private readonly CaroDbContext _context;

    public ChatChannelServiceImpl(CaroDbContext context)
    {
        _context = context;
    }

 

    // ServiceResult kiểu trả về cho Service
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
        public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    }

    // Lấy channel để join
    public async Task<ChatChannel?> GetChannelForJoinAsync(int channelId)
    {
        return await _context.ChatChannels
            .Include(c => c.Members)
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == channelId);
    }

    // Join channel
    public async Task<ServiceResult<bool>> JoinChannelAsync(int channelId, int userId, string? password)
    {
        var channel = await _context.ChatChannels
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
            return ServiceResult<bool>.Fail("Kênh không tồn tại.");

        if (channel.VoiceEnabled && channel.MaxMembers == 0)
            return ServiceResult<bool>.Fail("Đây là voice channel, không thể join bằng chat.");

        if (channel.Members.Any(m => m.UserId == userId))
            return ServiceResult<bool>.Fail("Bạn đã là thành viên của kênh.");

        if (channel.CreatorId == userId)
            return ServiceResult<bool>.Ok(true); // Creator luôn được coi là trong kênh

        // Trường hợp kênh yêu cầu invite (RequireInvite = true)
        if (channel.RequireInvite)
        {
            var existingInvite = await _context.ChannelInvites
                .FirstOrDefaultAsync(i => i.ChannelId == channelId && i.UserId == userId && !i.Accepted && !i.Rejected);

            if (existingInvite != null)
                return ServiceResult<bool>.Fail("Bạn đã gửi yêu cầu tham gia kênh này.");

            var newInvite = new ChannelInvite
            {
                ChannelId = channelId,
                UserId = userId,
                InvitedById = channel.CreatorId, // Creator là người nhận request
                CreatedAt = DateTime.UtcNow,
                Accepted = false,
                Rejected = false
            };

            _context.ChannelInvites.Add(newInvite);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(false); // false = đang chờ admin duyệt
        }

        // Trường hợp kênh riêng (IsPrivate = true) → cần mật khẩu
        if (channel.IsPrivate && channel.PasswordHash != null)
        {
            if (password == null || !BCrypt.Net.BCrypt.Verify(password, channel.PasswordHash))
                return ServiceResult<bool>.Fail("Sai mật khẩu kênh.");
        }

        // Kiểm tra giới hạn thành viên
        if (channel.Members.Count >= channel.MaxMembers)
            return ServiceResult<bool>.Fail("Kênh đã đầy.");

        // Thêm user vào member
        channel.Members.Add(new ChannelMember
        {
            UserId = userId,
            ChannelId = channelId,
            JoinedAt = DateTime.UtcNow,
            IsModerator = false,
            IsMuted = false
        });

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true); // true = join thành công
    }

    // LEAVE
    public async Task<bool> LeaveChannelAsync(int channelId, int userId)
    {
        var channel = await _context.ChatChannels
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
            throw new Exception("Kênh không tồn tại.");

        // Creator không thể rời
        if (channel.CreatorId == userId)
            throw new Exception("Người tạo kênh không thể rời kênh.");

        var member = channel.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            throw new Exception("Bạn chưa tham gia kênh.");

        // Nếu là moderator cuối → không cho leave
        if (member.IsModerator && channel.Members.Count(m => m.IsModerator) == 1)
            throw new Exception("Bạn là moderator cuối cùng. Hãy chọn người thay thế trước khi rời kênh.");

        _context.ChannelMembers.Remove(member);
        await _context.SaveChangesAsync();

        return true;
    }

    // UPDATE CHANNEL
    public async Task<bool> UpdateChannelAsync(int channelId, ChatChannelUpdateDTO dto, int currentUserId)
    {
        var channel = await _context.ChatChannels.FindAsync(channelId);
        if (channel == null)
            throw new Exception("Kênh không tồn tại.");

        if (channel.CreatorId != currentUserId)
            throw new Exception("Bạn không có quyền cập nhật kênh.");

        if (!string.IsNullOrEmpty(dto.Name))
            channel.Name = dto.Name.Trim();

        if (!string.IsNullOrEmpty(dto.Description))
            channel.Description = dto.Description.Trim();

        if (dto.MaxMembers.HasValue && dto.MaxMembers < channel.Members.Count)
            throw new Exception("Giới hạn thành viên mới nhỏ hơn số thành viên hiện tại.");

        if (!string.IsNullOrEmpty(dto.Password))
            channel.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        if (dto.IsPrivate.HasValue)
            channel.IsPrivate = dto.IsPrivate.Value;

        if (dto.RequireInvite.HasValue)
            channel.RequireInvite = dto.RequireInvite.Value;

        if (dto.VoiceEnabled.HasValue)
            channel.VoiceEnabled = dto.VoiceEnabled.Value;

        if (dto.MaxMembers.HasValue)
            channel.MaxMembers = dto.MaxMembers.Value;
        // Tự động gán EventRefId = 2 khi update
        channel.EventRefId = 2;

        await _context.SaveChangesAsync();
        return true;
    }

    // DELETE
    public async Task<bool> DeleteChannelAsync(int channelId, int currentUserId)
    {
        var channel = await _context.ChatChannels
            .Include(c => c.VoiceChannel)
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
            throw new Exception("Kênh không tồn tại.");

        if (channel.CreatorId != currentUserId)
            throw new Exception("Bạn không có quyền xoá kênh.");

        // FIX: VoiceChannel không phải collection nên không dùng Any()
        if (channel.VoiceChannel != null)
            throw new Exception("Không thể xoá kênh vì vẫn còn voice channel con.");

        // Xóa luôn member
        _context.ChannelMembers.RemoveRange(channel.Members);

        _context.ChatChannels.Remove(channel);
        await _context.SaveChangesAsync();
        return true;
    }

    // GET PUBLIC
    public async Task<List<ChatChannelResponseDTO>> GetPublicChannelsAsync()
    {
        var channels = await _context.ChatChannels
            .Where(c => !c.IsPrivate)
            .Include(c => c.Members)
            .Include(c => c.Creator)
            .ToListAsync();

        return channels.Select(c => new ChatChannelResponseDTO(c)).ToList();
    }

    // GET BY ID 
    public async Task<ChatChannelResponseDTO?> GetChannelByIdAsync(int channelId, int? userId = null)
    {
        var channel = await _context.ChatChannels
            .Include(c => c.Members).ThenInclude(m => m.User)
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
            return null; // không tồn tại -> controller sẽ trả 404

        bool isMember = userId.HasValue && channel.Members.Any(m => m.UserId == userId.Value);
        bool isCreator = userId.HasValue && channel.CreatorId == userId.Value;

        // Nếu private → chỉ cho phép creator hoặc member
        if (channel.IsPrivate && !isMember && !isCreator)
            return null; // không có quyền -> controller trả 403

        return new ChatChannelResponseDTO(channel);
    }
    public async Task<List<ChannelInviteDTO>> GetInvitesForUserAsync(int currentUserId)
    {
        var invites = await _context.ChannelInvites
            .Include(i => i.Channel)
            .ThenInclude(c => c.Creator)
            .Include(i => i.User)
            .Include(i => i.InvitedBy)
            .Where(i => !i.Accepted && !i.Rejected &&
           (i.InvitedById == currentUserId ||
            i.Channel.CreatorId == currentUserId ||
            i.UserId == currentUserId))

            .ToListAsync();

        return invites.Select(i => new ChannelInviteDTO
        {
            Id = i.Id,
            ChannelId = i.ChannelId,
            ChannelName = i.Channel!.Name,
            ChannelDescription = i.Channel.Description,
            UserId = i.UserId,
            UserName = i.User!.Username,
            InvitedById = i.InvitedById,
            InvitedByName = i.InvitedBy!.Username,
            CreatedAt = i.CreatedAt,
            Accepted = i.Accepted,
            Rejected = i.Rejected
        }).ToList();
    }

    public async Task<ChatChannelResponseDTO> CreateChannelAsync(ChatChannelCreateDTO dto, int userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Tên kênh không được để trống.");

        if (dto.MaxMembers <= 1)
            throw new Exception("Channel phải có ít nhất 2 thành viên.");

        ChatChannel channel = new ChatChannel
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? "",
            IsPrivate = dto.IsPrivate,
            RequireInvite = dto.RequireInvite,
            VoiceEnabled = dto.VoiceEnabled,
            MaxMembers = dto.MaxMembers,
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = string.IsNullOrEmpty(dto.Password)
                           ? null
                           : BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        // Chỉ gán Event nếu dto.EventId > 0 và chưa tồn tại channel với EventRefId đó
        if (dto.EventId > 0)
        {
            bool exists = await _context.ChatChannels.AnyAsync(c => c.EventRefId == dto.EventId);
            if (exists)
                throw new Exception($"Channel cho event {dto.EventId} đã tồn tại.");

            var eventEntity = await _context.Events.FindAsync(dto.EventId);
            if (eventEntity == null)
                throw new Exception("Event không tồn tại.");

            channel.EventRefId = dto.EventId;
            channel.Event = eventEntity;
        }

        _context.ChatChannels.Add(channel);
        await _context.SaveChangesAsync();

        return await GetChannelByIdAsync(channel.Id);
    }

    public async Task<ServiceResult<bool>> AcceptInviteAsync(int inviteId, int currentUserId)
    {
        var invite = await _context.ChannelInvites
            .Include(i => i.Channel).ThenInclude(c => c.Members)
            .Include(i => i.User)
            .Include(i => i.InvitedBy)
            .FirstOrDefaultAsync(i => i.Id == inviteId);

        if (invite == null)
            return ServiceResult<bool>.Fail("Lời mời không tồn tại.");

        if (invite.Channel == null)
            return ServiceResult<bool>.Fail("Kênh liên quan không tồn tại.");

        if (invite.User == null)
            return ServiceResult<bool>.Fail("Người được mời không tồn tại.");

        if (invite.Channel.CreatorId != currentUserId)
            return ServiceResult<bool>.Fail("Bạn không có quyền duyệt lời mời.");

        if (invite.Accepted || invite.Rejected)
            return ServiceResult<bool>.Fail("Lời mời đã xử lý.");

        invite.Channel.Members.Add(new ChannelMember
        {
            UserId = invite.UserId,
            ChannelId = invite.ChannelId,
            JoinedAt = DateTime.UtcNow
        });

        invite.Accepted = true;

        // Gửi thông báo cho người gửi invite
        if (invite.InvitedBy != null)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = invite.InvitedById, // Người gửi invite nhận thông báo
                Message = $"Người dùng {invite.User.Username} đã chấp nhận lời mời tham gia kênh {invite.Channel.Name}.",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> RejectInviteAsync(int inviteId, int currentUserId)
    {
        var invite = await _context.ChannelInvites
            .Include(i => i.Channel)
            .Include(i => i.User)
            .Include(i => i.InvitedBy)
            .FirstOrDefaultAsync(i => i.Id == inviteId);

        if (invite == null)
            return ServiceResult<bool>.Fail("Lời mời không tồn tại.");

        if (invite.Channel == null)
            return ServiceResult<bool>.Fail("Kênh liên quan không tồn tại.");

        if (invite.User == null)
            return ServiceResult<bool>.Fail("Người được mời không tồn tại.");

        if (invite.Channel.CreatorId != currentUserId)
            return ServiceResult<bool>.Fail("Bạn không có quyền từ chối lời mời.");

        if (invite.Accepted || invite.Rejected)
            return ServiceResult<bool>.Fail("Lời mời đã xử lý.");

        invite.Rejected = true;

        if (invite.InvitedBy != null)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = invite.InvitedById,
                Message = $"Người dùng {invite.User.Username} đã từ chối lời mời tham gia kênh {invite.Channel.Name}.",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }


    public async Task<UserChannelSummaryDTO> GetUserChannelsAsync(int userId)
    {
        var channels = await _context.ChatChannels
            .Include(c => c.Members).ThenInclude(m => m.User)
            .Where(c => c.Members.Any(m => m.UserId == userId) || c.CreatorId == userId)
            .ToListAsync();

        var response = new UserChannelSummaryDTO
        {
            TotalChannels = channels.Count,
            Channels = channels.Select(c => new ChatChannelResponseDTO(c)).ToList()
        };

        return response;
    }



}
