using GlobalFests.Data;
using GlobalFests.EFModels;
using GlobalFests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Services
{
    public interface ITicketService
    {
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsByUserIdAsync(int userId);
        Task<List<Ticket>> GetTicketsByEventIdAsync(int eventId);
        Task<Ticket> PurchaseTicketAsync(int eventId, int userId, decimal price);
        Task<bool> CancelTicketAsync(int ticketId);



        //Checkout logic
        Task<int> GetUserTicketCountForEventAsync(int userId, int eventId);
        Task<bool> PurchaseTicketsAsync(int eventId, int userId, int quantity);
        Task<bool> CancelTicketAsync(int ticketId, int userId);
    }
    public class TicketService : ITicketService
    {
        private readonly ICRUD<Ticket> _ticketRepository;
        private readonly GlobalFestsContext _context;

        public TicketService(ICRUD<Ticket> ticketRepository, GlobalFestsContext context)
        {
            _ticketRepository = ticketRepository;
            _context = context;
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(int userId)
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Where(t => t.UserId == userId).ToList();
        }

        public async Task<List<Ticket>> GetTicketsByEventIdAsync(int eventId)
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Where(t => t.EventId == eventId).ToList();
        }

        public async Task<Ticket> PurchaseTicketAsync(int eventId, int userId, decimal price)
        {
            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                Price = price,
                CreatedAt = DateTime.Now
            };

            return await _ticketRepository.CreateAsync(ticket);
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            return await _ticketRepository.DeleteAsync(ticketId);
        }



        public async Task<int> GetUserTicketCountForEventAsync(int userId, int eventId)
        {
            return await _context.Tickets
                .CountAsync(t => t.UserId == userId && t.EventId == eventId);
        }

        public async Task<bool> PurchaseTicketsAsync(int eventId, int userId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                var eventItem = await _context.Events.FindAsync(eventId);
                if (eventItem == null) return false;


                if (eventItem.TicketAmount < quantity) return false; 

                
                for (int i = 0; i < quantity; i++)
                {
                    var ticket = new Ticket
                    {
                        EventId = eventId,
                        UserId = userId,
                        Price = eventItem.TicketPrice ?? 0,
                        CreatedAt = DateTime.Now
                    };
                    _context.Tickets.Add(ticket);
                }

                // if i will choose to work without triggers 
                // eventItem.TicketAmount -= quantity; 

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> CancelTicketAsync(int ticketId, int userId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            // check ticket exists and belongs to the user requesting cancellation
            if (ticket == null || ticket.UserId != userId) return false;

            // Check if event already started (can't cancel past events)
            var ev = await _context.Events.FindAsync(ticket.EventId);
            if (ev != null && ev.StartDate < DateTime.Now) return false;

            _context.Tickets.Remove(ticket);

           
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
