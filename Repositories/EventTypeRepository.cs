using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class EventTypeRepository : ICRUD<EventType>
    {
        private readonly DbContext _context;

        public EventTypeRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<EventType> CreateAsync(EventType entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<EventType>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<EventType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<EventType>()
                .Include(e => e.Events)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<EventType>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<EventType>().Include(e => e.Events);

            if (trackChanges)
                return await query.ToListAsync(cancellationToken);
            
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<EventType> UpdateAsync(EventType entity, CancellationToken cancellationToken = default)
        {
            _context.Set<EventType>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<EventType>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<EventType>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
