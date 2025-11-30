using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class PermissionRepository : ICRUD<Permission>
    {
        private readonly DbContext _context;

        public PermissionRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Permission> CreateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Permission>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Permission>()
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Permission>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Permission>().Include(p => p.Roles);

            if (!trackChanges)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Permission, ICollection<Role>>)query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Permission> UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Permission>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Permission>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Permission>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
