using GlobalFests.DTOs;
using GlobalFests.EFModels;

namespace GlobalFests.Repositories
{
    public interface IEventRepository : ICRUD<Event>
    {
        Task<Event?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Event>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

        // with dto
        Task<EventDto?> GetEventDtoByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<EventDto>> GetAllEventDtosAsync(CancellationToken cancellationToken = default);
        Task<EventWithDetailsDto?> GetEventWithDetailsDtoAsync(int id, CancellationToken cancellationToken = default);

        // pagination
        Task<CursorResult<EventDto>> GetEventsByCursorAsync(
             DateTime? cursorDate,
             int? cursorId,
             int pageSize,
             CancellationToken cancellationToken = default);

        Task<CursorResult<EventOrganizerDto>> GetAllEventsByOrganizerAsync(
           int organizerId,
           DateTime? cursorDate,
           int? cursorId,
           int pageSize,
           CancellationToken cancellationToken = default);

        // grouping 
        //Task<List<EventsByCountryDto>> GetEventsByCountryAsync(CancellationToken cancellationToken = default);
        //Task<List<EventsByTypeDto>> GetEventsByTypeAsync(CancellationToken cancellationToken = default);

        // filtering
        Task<CursorResult<T>> SearchEventsAsync<T>(
            string? title = null,
            string? city = null,
            int? countryId = null,
            int? typeId = null,
            int? genreId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            int? status = null,
            DateTime? cursorDate = null, 
            int? cursorId = null,       
            int pageSize = 15,           
            CancellationToken cancellationToken = default);
    }
}
