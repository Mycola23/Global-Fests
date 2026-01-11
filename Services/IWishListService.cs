using GlobalFests.DTOs;

namespace GlobalFests.Services
{
    public interface IWishListService
    {
        Task ToggleWishListAsync(int userId, int eventId);
        Task<List<EventDto>> GetUserWishListAsync(int userId);
        Task<bool> IsInWishListAsync(int userId, int eventId);
    }
}
