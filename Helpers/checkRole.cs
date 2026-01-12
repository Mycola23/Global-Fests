using System.ComponentModel.DataAnnotations;
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

    public enum SortState
    {
        [Display(Name = "Date: Newest First")]
        DateDesc = 1,
        [Display(Name = "Date: Oldest First")]
        DateAsc = 2,
        [Display(Name = "Price: Low to High")]
        PriceAsc = 3,
        [Display(Name = "Price: High to Low")]
        PriceDesc = 4,
        [Display(Name = "Popular")]
        PopularitySales = 5
    }
}
