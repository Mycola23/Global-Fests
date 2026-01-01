using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class PerformerRepository : IPerformerRepository
    {
        private readonly DbContext _context;

        public PerformerRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Performer> CreateAsync(Performer entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Performer>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Performer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Performer>()
                .Include(p => p.Country)
                .Include(p => p.Events)
                .Include(p => p.Genres)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Performer>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Performer>()
                .Include(p => p.Country)
                .Include(p => p.Events);

            if (trackChanges)
                return await query.ToListAsync(cancellationToken);
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Performer> UpdateAsync(Performer entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Performer>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Performer>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Performer>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


        public async Task<CursorResult<PerformerDto>> GetAllPerformersByOrganizerAsync(
           int organizerId,
           DateTime? cursorDate = null,
           int? cursorId = null,
           int pageSize = 10,
           CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Performer>()
                .Include(e => e.Genres)
                .Include(e => e.Creator)
                .Where(e => e.CreatedBy == organizerId)
                .AsNoTracking()
                .AsQueryable();

            //  cursor logic
            if (cursorDate.HasValue && cursorId.HasValue)
            {
                query = query.Where(e => e.CreatedAt < cursorDate.Value
                                      || (e.CreatedAt == cursorDate.Value && e.Id < cursorId.Value));
            }
            var sql = query.ToQueryString();
            Console.WriteLine(sql);
            //  sorting limit
            var performers = await query
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return CreateCursorOrganizerResult(performers, pageSize);
        }

        public async Task<CursorResult<PerformerDto>> SearchPerformersAsync(
    string? searchTerm,
    int? status,
    DateTime? cursorDate,
    int? cursorId,
    int pageSize)
        {
            var query = _context.Set<Performer>()
                .Include(p => p.Genres)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (cursorDate.HasValue && cursorId.HasValue)
            {
                query = query.Where(p => p.CreatedAt < cursorDate.Value
                                      || (p.CreatedAt == cursorDate.Value && p.Id < cursorId.Value));
            }

            var entities = await query
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Take(pageSize)
                .ToListAsync();

            return CreateCursorOrganizerResult(entities, pageSize); 
        }

        private CursorResult<PerformerDto> CreateCursorOrganizerResult(List<Performer> performers, int pageSize)
        {
            var dtos = performers.Select(e => new PerformerDto
            {
                Id = e.Id,
                Name = e.Name,
                Avatar = e.Avatar,
                CreatedAt = e.CreatedAt,
                Status = e.Status,
                RejectionReason = e.RejectionReason,
                Genres = e.Genres.Select(g => new Genre
                {
                    Id = g.Id,
                    Genre1 = g.Genre1
                }).ToList()


            }).ToList();

            var result = new CursorResult<PerformerDto>
            {
                Items = dtos,
                HasNextPage = dtos.Count == pageSize
            };

            if (dtos.Any())
            {
                var lastItem = dtos.Last();
                result.NextCursorDate = lastItem.CreatedAt;
                result.NextCursorId = lastItem.Id;
            }

            return result;
        }

        public async Task<PerformerWithDetailsDto?> GetPerformerWithDetailsAsync(int id)
        {
            return await _context.Set<Performer>()
                .Where(p => p.Id == id)
                .Select(p => new PerformerWithDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Avatar = p.Avatar,
                    CountryName = p.Country != null ? p.Country.CountryName : "Unknown",
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    RejectionReason = p.RejectionReason,
                    CreatorName = p.Creator != null ? p.Creator.Username : "Unknown",
                    Genres = p.Genres.ToList(),
                    Events = p.Events.Where(e => e.Status == (int)Status.Approved) 
                                   .Select(e => new PerformerEventDto
                                   {
                                       Id = e.Id,
                                       Title = e.Title,
                                       StartDate = e.StartDate,
                                       City = e.City,
                                       Poster = e.Poster
                                   }).OrderBy(e => e.StartDate).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
