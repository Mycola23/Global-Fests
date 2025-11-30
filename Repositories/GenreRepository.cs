using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class GenreRepository : ICRUD<Genre>
    {
        private readonly DbContext _context;

        public GenreRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Genre> CreateAsync(Genre entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Genre>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Genre?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Genre>()
                .Include(g => g.Events)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<List<Genre>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Genre>().Include(g => g.Events);

            if (!trackChanges)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Genre, ICollection<Event>>)query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Genre> UpdateAsync(Genre entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Genre>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Genre>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Genre>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
