using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Repositories;

namespace GlobalFests.Services
{
    public interface IEventService
    {
        Task<EventDto?> GetEventByIdAsync(int id);
        Task<EventWithDetailsDto?> GetEventWithDetailsAsync(int id);
        Task<List<EventDto>> GetAllEventsAsync();
        Task<CursorResult<EventDto>> GetEventsPaginatedAsync(DateTime? cursorDate, int? cursorId, int pageSize = 15);
        Task<CursorResult<EventDto>> SearchEventsAsync(
            string? title, string? city, int? countryId, int? typeId,
            decimal? minPrice, decimal? maxPrice, DateTime? startDateFrom,
            DateTime? startDateTo, int? status, DateTime? cursorDate,
            int? cursorId, int pageSize = 15);
        Task<Event> CreateEventAsync(Event eventEntity);
        Task<Event> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetEventDtoByIdAsync(id);
        }

        public async Task<EventWithDetailsDto?> GetEventWithDetailsAsync(int id)
        {
            return await _eventRepository.GetEventWithDetailsDtoAsync(id);
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventDtosAsync();
        }

        public async Task<CursorResult<EventDto>> GetEventsPaginatedAsync(
            DateTime? cursorDate, int? cursorId, int pageSize = 15)
        {
            return await _eventRepository.GetEventsByCursorAsync(cursorDate, cursorId, pageSize);
        }

        public async Task<CursorResult<EventDto>> SearchEventsAsync(
            string? title, string? city, int? countryId, int? typeId,
            decimal? minPrice, decimal? maxPrice, DateTime? startDateFrom,
            DateTime? startDateTo, int? status, DateTime? cursorDate,
            int? cursorId, int pageSize = 15)
        {
            return await _eventRepository.SearchEventsAsync(
                title, city, countryId, typeId, minPrice, maxPrice,
                startDateFrom, startDateTo, status, cursorDate, cursorId, pageSize);
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));
            return await _eventRepository.CreateAsync(eventEntity);
        }

        public async Task<Event> UpdateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));

            return await _eventRepository.UpdateAsync(eventEntity);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            return await _eventRepository.DeleteAsync(id);
        }
    }
}