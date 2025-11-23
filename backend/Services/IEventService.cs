using backend.DTOs.Event;
using backend.DTOs.User;
using backend.Models;

namespace backend.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEventsAsync(UserResponseDTO currentUser);
        Task<EventDTO?> GetEventByIdAsync(int id);
        Task<EventDTO> CreateEventAsync(EventCreateDTO dto);
        Task<EventDTO?> UpdateEventAsync(int id, EventUpdateDTO dto);
        Task<bool> DeleteEventAsync(int id);
    }
}
