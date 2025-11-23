
using backend.DTOs.Event;
using backend.DTOs.User;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class EventServiceImpl : IEventService
    {
        private readonly CaroDbContext _context;

        public EventServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsAsync(UserResponseDTO currentUser)
        {
            var roleId = currentUser.RoleId;

            var query = _context.Events.AsQueryable();

            if (roleId == 3)
            {
                // Chỉ lấy ChatChannel và ClanRecruit
                query = query.Where(e => e.Type == EventType.ChatChannel || e.Type == EventType.ClanRecruit);
            }

            return await query
                .Select(e => new EventDTO
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Type = e.Type,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();
        }


        public async Task<EventDTO?> GetEventByIdAsync(int id)
        {
            var ev = await _context.Events.Include(f=>f.Features).FirstOrDefaultAsync(f=>f.EventId==id);
            if (ev == null) return null;

            return new EventDTO
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Type = ev.Type,
                CreatedAt = ev.CreatedAt,
                Features = ev.Features.Select(c => c.Name).ToList()
            };
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO dto)
        {
            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return new EventDTO
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Type = ev.Type,
                CreatedAt = ev.CreatedAt
            };
        }

        public async Task<EventDTO?> UpdateEventAsync(int id, EventUpdateDTO dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return null;

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Type = dto.Type;

            await _context.SaveChangesAsync();

            return new EventDTO
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Type = ev.Type,
                CreatedAt = ev.CreatedAt
            };
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return false;

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
