using GlobalFests.Data;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Services
{
    public class WishListService: IWishListService
    {
        private readonly GlobalFestsContext _context;

        public WishListService(GlobalFestsContext context)
        {
            _context = context;
        }

        public async Task ToggleWishListAsync(int userId, int eventId)
        {
            var existingItem = await _context.WishList
                .FirstOrDefaultAsync(w => w.UserId == userId && w.EventId == eventId);

            if (existingItem != null)
            {
                _context.WishList.Remove(existingItem); 
            }
            else
            {
                await _context.WishList.AddAsync(new WishList
                {
                    UserId = userId,
                    EventId = eventId
                }); 
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsInWishListAsync(int userId, int eventId)
        {
            return await _context.WishList
                .AnyAsync(w => w.UserId == userId && w.EventId == eventId);
        }

        public async Task<List<EventDto>> GetUserWishListAsync(int userId)
        {
           
            var events = await _context.WishList
                .Where(w => w.UserId == userId)
                .Include(w => w.Event).ThenInclude(e => e.Country)
                .Include(w => w.Event).ThenInclude(e => e.Organizer)
                .Select(w => w.Event)
                .ToListAsync();

           
            return events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Poster = e.Poster,
                StartDate = e.StartDate,
                TicketPrice = e.TicketPrice,
                City = e.City ?? "N/A",
                CountryName = e.Country.CountryName,
                OrganizerName = e.Organizer.Username
            }).ToList();
        }
    }
}
