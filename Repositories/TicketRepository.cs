using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class TicketRepository : ICRUD<Ticket>
    {
        private readonly DbContext _context;

        public TicketRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> CreateAsync(Ticket entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<Ticket>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Ticket>()
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Ticket>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Ticket>()
                .Include(t => t.Event)
                .Include(t => t.User);

            if (!trackChanges)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ticket, User>)query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Ticket> UpdateAsync(Ticket entity, CancellationToken cancellationToken = default)
        {
            _context.Set<Ticket>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Ticket>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<Ticket>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
