using backend.DTOs;
using backend.DTOs.Event;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService; // Inject IUserService

        public EventController(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            // Lấy userId từ JWT claim
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Lấy thông tin user từ IUserService
            var currentUser = await _userService.GetByIdAsync(userId);
            if (currentUser == null) return Unauthorized();

            // currentUser là DTO, bạn cần map RoleName nếu chưa có
            var events = await _eventService.GetAllEventsAsync(currentUser);

            return Ok(events);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateDTO dto)
        {
            var created = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.EventId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EventUpdateDTO dto)
        {
            var updated = await _eventService.UpdateEventAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
