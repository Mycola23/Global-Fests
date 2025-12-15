using System.Security.Claims;

namespace GlobalFests.Helpers
{
    public enum UserRole
    {
        Admin = 1,
        User = 2,
        Organizer = 3
    }
    public static class checkRole
    {
        public static bool HasRole(this ClaimsPrincipal user, UserRole role)
        {
            // Перевіряємо, чи є роль "1", "2" і т.д.
            return user.IsInRole(((int)role).ToString());
        }
    }
}
