using backend.DTOs.ChatMessage;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatMessageController : ControllerBase
{
    private readonly IChatMessageService _service;

    public ChatMessageController(IChatMessageService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ChatMessageResponseDTO>> SendMessage([FromBody] ChatMessageCreateDTO dto)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var message = await _service.SendMessageAsync(dto, userId);
        return Ok(message);
    }

    [HttpGet("channel/{channelId}")]
    [Authorize]
    public async Task<ActionResult<List<ChatMessageResponseDTO>>> GetMessages(int channelId)
    {
        var messages = await _service.GetMessagesByChannelAsync(channelId);
        return Ok(messages);
    }

    [HttpPost("{messageId}/react")]
    [Authorize]
    public async Task<ActionResult> ReactToMessage(int messageId, [FromBody] string emoji)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var success = await _service.ReactToMessageAsync(messageId, userId, emoji);
        if (!success) return BadRequest("Không thể react tin nhắn.");
        return Ok("React thành công.");
    }
}
