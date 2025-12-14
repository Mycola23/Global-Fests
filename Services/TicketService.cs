using GlobalFests.EFModels;
using GlobalFests.Repositories;

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
    }
    public class TicketService : ITicketService
    {
        private readonly ICRUD<Ticket> _ticketRepository;

        public TicketService(ICRUD<Ticket> ticketRepository)
        {
            _ticketRepository = ticketRepository;
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
    }
}
