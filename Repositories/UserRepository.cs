using GlobalFests.Data;
using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class UserRepository : ICRUD<User>
    {
        private readonly DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }


        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<User>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }


        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }


        public async Task<List<User>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Country);

            if (!trackChanges)
            {
                return await query.AsNoTracking().ToListAsync(cancellationToken);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var trackedEntity = _context.ChangeTracker.Entries<User>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (trackedEntity == null)
            {

                _context.Set<User>().Update(entity);
            }


            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Set<User>().FindAsync(new object[] { id }, cancellationToken);

            if (user == null)
                return false;

            _context.Set<User>().Remove(user);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
