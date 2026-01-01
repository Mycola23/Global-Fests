using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class RoleRepository : ICRUD<Role>
    {
        private readonly DbContext _context;

        public RoleRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Role> CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Role>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Role>()
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<List<Role>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Role>()
                .Include(r => r.Users);

            if (trackChanges)
                return await query.ToListAsync(cancellationToken);
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Role>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Role>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Role>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
