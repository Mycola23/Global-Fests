using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class PerformerRepository : ICRUD<Performer>
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
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Performer>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Performer>()
                .Include(p => p.Country)
                .Include(p => p.Events);

            if (!trackChanges)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Performer, ICollection<Event>>)query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
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
    }
}
