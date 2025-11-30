using GlobalFests.Data;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly GlobalFestsContext _context;

        public EventRepository(GlobalFestsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Event> CreateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<Event>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<Event?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Include(e => e.Organizer)
                .Include(e => e.Country)
                .Include(e => e.Type)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<Event>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Event>()
                .Include(e => e.Organizer)
                .Include(e => e.Country)
                .Include(e => e.Type);

            if (!trackChanges)
            {
                return await query.AsNoTracking().ToListAsync(cancellationToken);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Event> UpdateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var trackedEntity = _context.ChangeTracker.Entries<Event>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (trackedEntity == null)
            {
                _context.Set<Event>().Update(entity);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var eventEntity = await _context.Set<Event>().FindAsync(new object[] { id }, cancellationToken);

            if (eventEntity == null)
                return false;

            _context.Set<Event>().Remove(eventEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }




        public async Task<Event?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Include(e => e.Organizer)                    // load
                    .ThenInclude(o => o.Role)                 // inside Include 
                .Include(e => e.Country)
                .Include(e => e.Type)
                .Include(e => e.Genres)                       // load  genres (many-to-many)
                .Include(e => e.Performers)                   // load performers (many-to-many)
                    .ThenInclude(p => p.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<Event>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Include(e => e.Organizer)
                    .ThenInclude(o => o.Role)
                .Include(e => e.Country)
                .Include(e => e.Type)
                .Include(e => e.Genres)
                .Include(e => e.Performers)
                    .ThenInclude(p => p.Country)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }



       

        //   DTO - get only necessary fields (auto AsNoTracking)
        public async Task<EventDto?> GetEventDtoByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Where(e => e.Id == id)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    City = e.City ?? "N/A",
                    CountryName = e.Country.CountryName,
                    EventType = e.Type.Type,
                    OrganizerName = e.Organizer.Username
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<EventDto>> GetAllEventDtosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    City = e.City ?? "N/A",
                    CountryName = e.Country.CountryName,
                    EventType = e.Type.Type,
                    OrganizerName = e.Organizer.Username
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<EventWithDetailsDto?> GetEventWithDetailsDtoAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Event>()
                .Where(e => e.Id == id)
                .Select(e => new EventWithDetailsDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    TicketAmount = e.TicketAmount,
                    City = e.City,
                    Address = e.Address,
                    CountryName = e.Country.CountryName,
                    EventType = e.Type.Type,
                    OrganizerName = e.Organizer.Username,
                    OrganizerEmail = e.Organizer.Email,
                    Genres = e.Genres.Select(g => g.Genre1).ToList(),
                    Performers = e.Performers.Select(p => p.Name).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }





        public async Task<CursorResult<EventDto>> GetEventsByCursorAsync(
           DateTime? cursorDate,
           int? cursorId,
           int pageSize,
           CancellationToken cancellationToken = default)
        {
            // Використовуємо Raw SQL, як ви і хотіли
            // MSSQL синтаксис: TOP замість LIMIT, розгорнута логіка OR
            var query = _context.Events
                .FromSqlInterpolated($@"
                    SELECT TOP({pageSize}) *
                    FROM Events
                    WHERE {cursorDate} IS NULL 
                       OR StartDate < {cursorDate} 
                       OR (StartDate = {cursorDate} AND Id < {cursorId})
                    ORDER BY StartDate DESC, Id DESC
                ")
                .Include(e => e.Organizer)
                .Include(e => e.Country)
                .Include(e => e.Type)
                .AsNoTracking();

            var events = await query.ToListAsync(cancellationToken);

            // Мапимо в DTO та формуємо результат курсора
            return CreateCursorResult(events, pageSize);
        }

        // ==========================================
        // РЕАЛІЗАЦІЯ ПОШУКУ З КУРСОРОМ (LINQ Seek Method)
        // ==========================================
        public async Task<CursorResult<EventDto>> SearchEventsAsync(
            string? title = null,
            string? city = null,
            int? countryId = null,
            int? typeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            bool? approved = null,
            DateTime? cursorDate = null,
            int? cursorId = null,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Event>()
                .Include(e => e.Organizer)
                .Include(e => e.Country)
                .Include(e => e.Type)
                .AsNoTracking()
                .AsQueryable();

            // 1. Застосовуємо фільтри
            if (!string.IsNullOrWhiteSpace(title)) query = query.Where(e => e.Title.Contains(title));
            if (!string.IsNullOrWhiteSpace(city)) query = query.Where(e => e.City != null && e.City.Contains(city));
            if (countryId.HasValue) query = query.Where(e => e.CountryId == countryId.Value);
            if (typeId.HasValue) query = query.Where(e => e.TypeId == typeId.Value);
            if (minPrice.HasValue) query = query.Where(e => e.TicketPrice >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(e => e.TicketPrice <= maxPrice.Value);
            if (startDateFrom.HasValue) query = query.Where(e => e.StartDate >= startDateFrom.Value);
            if (startDateTo.HasValue) query = query.Where(e => e.StartDate <= startDateTo.Value);
            if (approved.HasValue) query = query.Where(e => e.Approved == approved.Value);

            // 2. Застосовуємо логіку курсора (Seek Pagination) через LINQ
            // Це компілюється в такий самий ефективний SQL, як і Raw SQL вище
            if (cursorDate.HasValue && cursorId.HasValue)
            {
                query = query.Where(e => e.StartDate < cursorDate.Value
                                      || (e.StartDate == cursorDate.Value && e.Id < cursorId.Value));
            }

            // 3. Сортування та ліміт
            var events = await query
                .OrderByDescending(e => e.StartDate)
                .ThenByDescending(e => e.Id)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return CreateCursorResult(events, pageSize);
        }

        // Допоміжний метод для створення відповіді
        private CursorResult<EventDto> CreateCursorResult(List<Event> events, int pageSize)
        {
            var dtos = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                TicketPrice = e.TicketPrice,
                City = e.City ?? "N/A",
                CountryName = e.Country?.CountryName ?? "Unknown",
                EventType = e.Type?.Type ?? "Unknown",
                OrganizerName = e.Organizer?.Username ?? "Unknown"
            }).ToList();

            var result = new CursorResult<EventDto>
            {
                Items = dtos,
                HasNextPage = dtos.Count == pageSize
            };

            if (dtos.Any())
            {
                var lastItem = dtos.Last();
                result.NextCursorDate = lastItem.StartDate;
                result.NextCursorId = lastItem.Id;
            }

            return result;
        }
    }
}
