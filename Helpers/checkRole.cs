using System.Security.Claims;

namespace GlobalFests.Helpers
{
    public enum UserRole
    {
        Admin = 1,
        User = 3,
        Organizer = 2,
        SuperAdmin =4,
    }

    public enum SearchWorkMode
    {
        Default = 1,
        WorldMap = 2,
    }
    public static class checkRole
    {
        public static bool HasRole(this ClaimsPrincipal user, UserRole role)
        {
            // Перевіряємо, чи є роль "1", "2" і т.д.
            return user.IsInRole(((int)role).ToString());
        }
    }

    public enum Status
    {
        Draft = 0,          // draft only org can view
        Pending = 1,        // on moderation
        Approved = 2,       // 
        Rejected = 3,       // need to be rewrited
        Archived = 4        // 
    }

    public enum AdminManageItemType
    {
        Genres,
        EventTypes,
        Roles,
    }
}
