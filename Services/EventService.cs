using GlobalFests.Data;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
using GlobalFests.Repositories;
using GlobalFests.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Services
{
    public interface IEventService
    {
        Task<EventDto?> GetEventByIdAsync(int id);
        Task<EventWithDetailsDto?> GetEventWithDetailsAsync(int id);
        Task<List<EventDto>> GetAllEventsAsync();
        Task<CursorResult<EventDto>> GetEventsPaginatedAsync(DateTime? cursorDate, int? cursorId, int pageSize = 15);
        Task<CursorResult<T>> SearchEventsAsync<T>(
            string? title, string? city, int? countryId, int? typeId, int? genreId,
            decimal? minPrice, decimal? maxPrice, DateTime? startDateFrom,
            DateTime? startDateTo, int? status, DateTime? cursorDate,
            int? cursorId, int pageSize = 15);
        Task<Event> CreateEventAsync(Event eventEntity);
        Task<Event> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);

        Task<HomeViewModel> GetHomePageDataAsync(int? currentUserId);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly GlobalFestsContext _context;

        public EventService(IEventRepository eventRepository, GlobalFestsContext context)
        {
            _eventRepository = eventRepository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public async Task<CursorResult<T>> SearchEventsAsync<T>(
            string? title, string? city, int? countryId, int? typeId, int? genreId,
            decimal? minPrice, decimal? maxPrice, DateTime? startDateFrom,
            DateTime? startDateTo, int? status, DateTime? cursorDate,
            int? cursorId, int pageSize = 15)
        {
            return await _eventRepository.SearchEventsAsync<T>(
                title, city, countryId, typeId,genreId, minPrice, maxPrice,
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



        public async Task<HomeViewModel> GetHomePageDataAsync(int? currentUserId)
        {
            var model = new HomeViewModel();
            var now = DateTime.Now;

            model.SearchResult = await SearchEventsAsync<EventDto>(
                title: null,
                city: null,
                countryId: null,
                typeId: null,
                genreId: null,
                minPrice: null,
                maxPrice: null,
                startDateFrom: null,
                startDateTo: null,
                status: (int)Status.Approved,
                cursorDate: null,
                cursorId: null,
                pageSize: 12);

            // trends upcoming bestselling local  
            var trending = await _context.Events
                .Include(e => e.Country).Include(e => e.Type).Include(e => e.Organizer)
                .Where(e => e.Status == (int)Status.Approved && e.StartDate > now)
                .OrderBy(x => Guid.NewGuid()) 
                .Take(10)
                .ToListAsync();

             
            var upcoming = await _context.Events
                .Include(e => e.Country).Include(e => e.Type).Include(e => e.Organizer)
                .Where(e => e.Status == (int)Status.Approved && e.StartDate > now)
                .OrderBy(e => e.StartDate)
                .Take(10)
                .ToListAsync();

            var bestSelling = await _context.Events
                .Include(e => e.Country).Include(e => e.Type).Include(e => e.Organizer).Include(e => e.Tickets)
                .Where(e => e.Status == (int)Status.Approved && e.StartDate > now)
                .OrderByDescending(e => e.Tickets.Count()) 
                .Take(10)
                .ToListAsync();

            model.BestSellingEvents = MapToDto(bestSelling);
            if (currentUserId.HasValue)
            {
                var user = await _context.Users
                    .Include(u => u.Country)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId.Value);

                if (user?.CountryId != null)
                {
                    model.UserCountryName = user.Country.CountryName;

                    var local = await _context.Events
                        .Include(e => e.Country).Include(e => e.Type).Include(e => e.Organizer)
                        .Where(e => e.Status == (int)Status.Approved
                                    && e.StartDate > now
                                    && e.CountryId == user.CountryId)
                        .OrderBy(e => e.StartDate)
                        .Take(10)
                        .ToListAsync();

                    
                    model.LocalEvents = MapToDto(local);
                }
            }

            model.TrendingEvents = MapToDto(trending);
            model.UpcomingEvents = MapToDto(upcoming);

            return model;
        }

       // helper
        private List<EventDto> MapToDto(List<Event> events)
        {
            return events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Poster = e.Poster,
                StartDate = e.StartDate,
                TicketPrice = e.TicketPrice,
                City = e.City ?? "N/A",
                CountryName = e.Country?.CountryName ?? "",
                OrganizerName = e.Organizer?.Username ?? "",
                Status = e.Status,
            }).ToList();
        }
    }
}