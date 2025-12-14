using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class CountryRepository : ICRUD<Country>
    {
        private readonly DbContext _context;

        public CountryRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Country> CreateAsync(Country entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Country>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Country>()
                .Include(c => c.Events)
                .Include(c => c.Performers)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<Country>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Country>()
                .Include(c => c.Events)
                .Include(c => c.Performers)
                .Include(c => c.Users);

            if (trackChanges)
            {
                return await query.ToListAsync(cancellationToken);
            }
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Country> UpdateAsync(Country entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Country>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Country>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Country>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
