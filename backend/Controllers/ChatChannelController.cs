using backend.DTOs.ChatChannel;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatChannelController : ControllerBase
{
    private readonly IChatChannelService _service;
    private readonly CaroDbContext _context;

    public ChatChannelController(IChatChannelService service, CaroDbContext context)
    {
        _service = service;
        _context = context;
    }

    // Tạo kênh
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ChatChannelResponseDTO>> CreateChannel([FromBody] ChatChannelCreateDTO dto)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var channel = await _service.CreateChannelAsync(dto, userId);
        return Ok(channel);
    }

    [HttpPost("{channelId}/join")]
    [Authorize]
    public async Task<ActionResult> JoinChannel(int channelId, [FromBody] string? password)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var channel = await _service.GetChannelForJoinAsync(channelId);
        if (channel == null)
            return NotFound("Kênh không tồn tại.");

        if (channel.CreatorId == userId)
            return BadRequest("Bạn là người tạo kênh, không cần tham gia.");

        var result = await _service.JoinChannelAsync(channelId, userId, password);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        if (!result.Data)
            return Ok("Yêu cầu tham gia kênh đã gửi, đang chờ admin duyệt.");

        return Ok("Tham gia kênh thành công.");
    }


    // Rời kênh
    [HttpPost("{channelId}/leave")]
    [Authorize]
    public async Task<ActionResult> LeaveChannel(int channelId)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var channel = await _service.GetChannelByIdAsync(channelId, userId);
        if (channel == null)
            return NotFound("Kênh không tồn tại.");

        if (channel.CreatorId == userId)
            return BadRequest("Bạn là người tạo kênh, không thể rời kênh.");

        var success = await _service.LeaveChannelAsync(channelId, userId);

        return success
            ? Ok("Rời kênh thành công.")
            : BadRequest("Bạn không phải thành viên của kênh.");
    }

    // Cập nhật kênh
    [HttpPut("{channelId}")]
    [Authorize]
    public async Task<ActionResult> UpdateChannel(int channelId, [FromBody] ChatChannelUpdateDTO dto)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var success = await _service.UpdateChannelAsync(channelId, dto, userId);

        return success
            ? Ok("Cập nhật kênh thành công.")
            : BadRequest("Cập nhật thất bại. Bạn không phải creator hoặc kênh không tồn tại.");
    }

    // Xóa kênh
    [HttpDelete("{channelId}")]
    [Authorize]
    public async Task<ActionResult> DeleteChannel(int channelId)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var success = await _service.DeleteChannelAsync(channelId, userId);

        return success
            ? Ok("Xóa kênh thành công.")
            : BadRequest("Xóa thất bại. Bạn không phải creator hoặc kênh không tồn tại.");
    }

    // Lấy danh sách public channel
    [HttpGet("public")]
    public async Task<ActionResult<List<ChatChannelResponseDTO>>> GetPublicChannels()
    {
        var channels = await _service.GetPublicChannelsAsync();
        return Ok(channels);
    }

    [HttpGet("{channelId}")]
    public async Task<ActionResult<ChatChannelResponseDTO>> GetChannelById(int channelId)
    {
        int? userId = null;
        if (User?.Identity?.IsAuthenticated == true)
            userId = int.Parse(User.FindFirst("userId")!.Value);

        var result = await _service.GetChannelByIdAsync(channelId, userId);

        if (result == null)
        {
            // kiểm tra tồn tại
            if (!await _context.ChatChannels.AnyAsync(c => c.Id == channelId))
                return NotFound("Kênh không tồn tại.");

            return StatusCode(403, "Bạn không có quyền xem thông tin kênh này.");
        }

        return Ok(result);
    }

    [HttpGet("invites")]
    [Authorize]
    public async Task<ActionResult<List<ChannelInviteDTO>>> GetMyInvites()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        // Gọi service trả về DTO
        var invites = await _service.GetInvitesForUserAsync(userId);

        // Trả về kết quả
        return Ok(invites);
    }


    [HttpPost("invites/{inviteId}/accept")]
    [Authorize]
    public async Task<ActionResult> AcceptInvite(int inviteId)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var result = await _service.AcceptInviteAsync(inviteId, userId);
        return result.Success ? Ok("Đã tham gia kênh.") : BadRequest(result.ErrorMessage);
    }

    [HttpPost("invites/{inviteId}/reject")]
    [Authorize]
    public async Task<ActionResult> RejectInvite(int inviteId)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var result = await _service.RejectInviteAsync(inviteId, userId);
        return result.Success ? Ok("Đã từ chối lời mời.") : BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpGet("my-channels")]
    public async Task<IActionResult> GetMyChannels()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var result = await _service.GetUserChannelsAsync(userId);

        return Ok(result);
    }



}
