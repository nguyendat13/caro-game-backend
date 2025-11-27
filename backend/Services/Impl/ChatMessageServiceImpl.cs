using backend.DTOs.ChatMessage;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Impl;

public class ChatMessageServiceImpl : IChatMessageService
{
    private readonly CaroDbContext _context;

    public ChatMessageServiceImpl(CaroDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMessageResponseDTO> SendMessageAsync(ChatMessageCreateDTO dto, int senderId)
    {
        var channel = await _context.ChatChannels
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == dto.ChannelId);

        if (channel == null) throw new Exception("Kênh không tồn tại.");

        if (!channel.Members.Any(m => m.UserId == senderId))
            throw new Exception("Bạn không phải thành viên của kênh này.");

        var message = new ChatMessage
        {
            ChannelId = dto.ChannelId,
            Content = dto.Content,
            SenderId = senderId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        // Load sender info
        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

        return new ChatMessageResponseDTO(message);
    }

    public async Task<List<ChatMessageResponseDTO>> GetMessagesByChannelAsync(int channelId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.ChannelId == channelId)
            .Include(m => m.Sender)
            .Include(m => m.Reactions)
            .ToListAsync();

        return messages.Select(m => new ChatMessageResponseDTO(m)).ToList();
    }

    public async Task<bool> ReactToMessageAsync(int messageId, int userId, string emoji)
    {
        var message = await _context.ChatMessages
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null) return false;

        if (!message.Reactions.Any(r => r.UserId == userId && r.Emoji == emoji))
        {
            message.Reactions.Add(new MessageReaction
            {
                UserId = userId,
                MessageId = messageId,
                Emoji = emoji
            });
            await _context.SaveChangesAsync();
        }

        return true;
    }
}
