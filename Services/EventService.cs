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
        Task<CursorResult<T>> SearchEventsAsync<T>(
            string? title, string? city, int? countryId, int? typeId, int? genreId,
            decimal? minPrice, decimal? maxPrice, DateTime? startDateFrom,
            DateTime? startDateTo, int? status, DateTime? cursorDate,
            int? cursorId, int pageSize = 15);
        Task<Event> CreateEventAsync(Event eventEntity);
        Task<Event> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);
        Task UpdateEventFullAsync(int id, EditEventsViewModel model, bool isOrganizer);
        Task<HomeViewModel> GetHomePageDataAsync(int? currentUserId);
        Task<bool> HasTicketsAsync(int eventId);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly GlobalFestsContext _context;
        private readonly ILookupService _lookupService;
        private readonly IPerformerRepository _performerRepo;

        public EventService(IEventRepository eventRepository, GlobalFestsContext context, ILookupService lookupService,
            IPerformerRepository performerRepo)
        {
            _eventRepository = eventRepository;
            _lookupService = lookupService;
            _performerRepo = performerRepo;
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

        public async Task UpdateEventFullAsync(int id, EditEventsViewModel model, bool isOrganizer)
        {
          
            var existingEvent = await _eventRepository.GetByIdAsync(id);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException($"Event with ID {id} not found.");
            }

            //business-checks
            bool hasSoldTickets = await HasTicketsAsync(id);
            bool eventStarted = existingEvent.StartDate <= DateTime.Now;
            bool eventEnded = existingEvent.EndDate <= DateTime.Now;

           
            if (isOrganizer && eventStarted && !eventEnded)
            {
                throw new InvalidOperationException("You cannot edit an event while it is in progress.");
            }

            
            if (hasSoldTickets)
            {
                if (existingEvent.TicketPrice != model.Event.TicketPrice)
                {
                    
                    throw new ArgumentException("Cannot change price because tickets have already been sold.", "Event.TicketPrice");
                }
                if (existingEvent.TicketAmount != model.Event.TicketAmount)
                {
                    throw new ArgumentException("Cannot change ticket quantity because sales have started.", "Event.TicketAmount");
                }
            }

            
            // mapping
            existingEvent.Title = model.Event.Title;
            existingEvent.Description = model.Event.Description;
            existingEvent.Poster = model.Event.Poster;
            existingEvent.TypeId = model.Event.TypeId;
            existingEvent.CountryId = model.Event.CountryId;
            existingEvent.City = model.Event.City;
            existingEvent.Address = model.Event.Address;
            existingEvent.Latitude = model.Event.Latitude;
            existingEvent.Longitude = model.Event.Longitude;

           
            if (!hasSoldTickets)
            {
                existingEvent.StartDate = model.Event.StartDate;
                existingEvent.EndDate = model.Event.EndDate;
                existingEvent.TicketPrice = model.Event.TicketPrice;
                existingEvent.TicketAmount = model.Event.TicketAmount;
            }

            if (isOrganizer)
            {
                existingEvent.Status = (int)Status.Pending;
            }

            //  update genres
            existingEvent.Genres.Clear();
            if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
            {
                foreach (var genreId in model.SelectedGenreIds)
                {
                    var genre = await _lookupService.GetGenreByIdAsync(genreId);
                    if (genre != null) existingEvent.Genres.Add(genre);
                }
            }

            //  update performers
            existingEvent.Performers.Clear();
            if (model.SelectedPerformerIds != null && model.SelectedPerformerIds.Any())
            {
                var selectedPerformers = await _performerRepo.GetPerformersByIdsAsync(model.SelectedPerformerIds);
                foreach (var performer in selectedPerformers)
                {
                    existingEvent.Performers.Add(performer);
                }
            }

            await _eventRepository.UpdateAsync(existingEvent);
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

        public async Task<bool> HasTicketsAsync(int eventId)
        {
            return await _context.Tickets.AnyAsync(t => t.EventId == eventId);
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