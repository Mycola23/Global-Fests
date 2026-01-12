using System.ComponentModel.DataAnnotations;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.ViewModels
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;

        
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "Choose your role,please")]
        public int? RoleId { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterUserModel User { get; set; } = new RegisterUserModel();

        [ValidateNever]
        public List<Country>? Countries { get; set; }
        [ValidateNever]
        public List<Role>? Roles { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }


    public class HomeViewModel
    {
        public CursorResult<EventDto>? SearchResult { get; set; }
        public List<EventDto> TrendingEvents { get; set; } = new();
        public List<EventDto> UpcomingEvents { get; set; } = new();
        public List<EventDto> BestSellingEvents { get; set; } = new();

        public List<EventDto>? LocalEvents { get; set; }
        public string? UserCountryName { get; set; }
    }
    public class EventsSearchModel
    {
        public string? Title { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? TypeId { get; set; }

        public int? GenreId { get; set; }

        public int? PriceSortingId { get; set; }

        public int? PopularityId { get; set; }

        [Display(Name = "Min Price")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Start Date From")]
        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; set; }

        [Display(Name = "Start Date To")]
        [DataType(DataType.Date)]
        public DateTime? StartDateTo { get; set; }

        public SortState SortOrder { get; set; }
    }

    public class EventsViewModel
    {
        public EventsSearchModel? Search { get; set; }

        public CursorSortingResult<EventDto> Events { get; set; }

        [ValidateNever]
        public List<Country>? Countries { get; set; }
        [ValidateNever]
        public List<EventType>? EventTypes { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; }
    }

    public class CreateEventsViewModel
    {
        public Event NewEvent { get; set; } = new Event();
        
        public List<Country>? Countries { get; set; }
        [ValidateNever]
        public List<EventType>? EventTypes { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; }


        [ValidateNever]
        public string? SearchPerformer { get; set; }
        [ValidateNever]
        public CursorResult<PerformerDto> Performers { get; set; } = new();
        public List<int> SelectedPerformerIds { get; set; } = new List<int>();

        public List<int> SelectedGenreIds { get; set; } = new List<int>();
    }

    // maybe later combine these two in one
    public class EditEventsViewModel
    {
        public Event Event { get; set; } = new Event();

        public List<Country>? Countries { get; set; }
        [ValidateNever]
        public List<EventType>? EventTypes { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        [ValidateNever]
        public string? SearchPerformer { get; set; }
        [ValidateNever]
        public CursorResult<PerformerDto> Performers { get; set; } = new();
        public List<int> SelectedPerformerIds { get; set; } = new List<int>();
    }

    public class EventDetailsViewModel
    {
        public EventWithDetailsDto Event { get; set; } = new EventWithDetailsDto();

        public List<ReviewViewDto> Reviews { get; set; } = new();

        public bool IsInWishList { get; set; }

    }




    public class CreatePerformerViewModel
    {
        
        public Performer NewPerformer { get; set; } = new Performer();
        [ValidateNever]
        public List<Country>? Countries { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; } 

        public List<int> SelectedGenreIds { get; set; } = new List<int>();
    }

    public class EditPerformerViewModel
    {

        public Performer Performer { get; set; } = new Performer();
        [ValidateNever]
        public List<Country>? Countries { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        public List<int> CurrentGenreIds { get; set; } = new List<int>();

    }

    public class PerformerDetailsViewModel
    {
        public PerformerWithDetailsDto Performer { get; set; } = new();
    }

    public class OrganizerPanelViewModel
    {
        public CursorResult<PerformerDto>? Performers { get; set; }

        public CursorResult<EventOrganizerDto>? Events { get; set; }

        public OrganizerPanelStats? Stats { get; set; }
    }

    public class AdminDashboardViewModel
    {
        // general stats
        public int TotalUsers { get; set; }
        public int TotalEvents { get; set; }
        public int TotalPerformers { get; set; }
        public decimal TotalRevenue { get; set; } 

        // queue on moderation for approve
        public List<Event> PendingEvents { get; set; } = new();
        public List<Performer> PendingPerformers { get; set; } = new();

        public AdminChartStats ChartStats { get; set; }
    }

    public class AdminChartStats
    {
        public List<MonthlyMetric> UserRegistrations { get; set; } = new();
        public List<CategoryMetric> EventsByType { get; set; } = new();
    }

    public class MonthlyMetric
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public class CategoryMetric
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }


    // for admin/table crud 

    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

  
        public int EventsCount { get; set; }

        public int PerformersCount { get; set; }
        public int UsersCount { get; set; }
    }

    public class CountryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100)]
        [Display(Name = "Country Name")]
        public string CountryName { get; set; } = null!;

        [Required(ErrorMessage = "ISO Code is required")]
        [StringLength(5, ErrorMessage = "Code cannot be longer than 5 chars")]
        [Display(Name = "ISO Code")]
        public string CountryCode { get; set; } = null!;
    }


    // universal veiwModel for genres,eventtypes,roles
    public class AdminManageItemViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public AdminManageItemType EntityType { get; set; } 
        public int? EventsCount { get; set; }
        public int? UsersCount { get; set; }
        public int? PerformersCount { get; set; }
    }


    // for users  in admin panel
    public class AdminUsersIndexViewModel
    {
        public CursorResult<UserDto> Users { get; set; }    
        public string? SearchTerm { get; set; }
    }
    public class AdminUserFormViewModel
    {
      
        public int Id { get; set; }
        [Required] public string Username { get; set; } = null!;

        [Required][EmailAddress][RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = null!;
        public string? NewPassword { get; set; }
        [Required] public int RoleId { get; set; }
        public int? CountryId { get; set; }
        public bool Verified { get; set; }
        public List<SelectListItem> Roles { get; set; } = new();
        public List<SelectListItem> Countries { get; set; } = new();
    }

    // for events in admin panel 

    public class AdminEventsIndexViewModel
    {
        public CursorResult<EventDto> Events { get; set; }
        // filters
        public string? SearchTerm { get; set; }
        public int? StatusFilter { get; set; } 
        public List<SelectListItem> Statuses { get; set; } = new()
        {
            new SelectListItem("All Statuses", ""),
            new SelectListItem("Draft", ((int)Status.Draft).ToString()),
            new SelectListItem("Pending", ((int)Status.Pending).ToString()),
            new SelectListItem("Approved", ((int)Status.Approved).ToString()),
            new SelectListItem("Rejected", ((int)Status.Rejected).ToString()),
            new SelectListItem("Archived", ((int)Status.Archived).ToString()),
        };
    }

    public class AdminPerformersIndexViewModel
    {
        public CursorResult<PerformerDto> Performers { get; set; } = new ();
        public string? SearchTerm { get; set; }

        public int? StatusFilter { get; set; }
        public List<SelectListItem> Statuses { get; set; } = new()
        {
            new SelectListItem("All Statuses", ""),
            new SelectListItem("Draft", ((int)Status.Draft).ToString()),
            new SelectListItem("Pending", ((int)Status.Pending).ToString()),
            new SelectListItem("Approved", ((int)Status.Approved).ToString()),
            new SelectListItem("Rejected", ((int)Status.Rejected).ToString()),
            new SelectListItem("Archived", ((int)Status.Archived).ToString()),
        };
    }

    // for cart and user history of buought tickets

    public class CheckoutViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = null!;
        public string EventPoster { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = null!;

        public decimal PricePerTicket { get; set; }

        [Range(1, 4, ErrorMessage = "You can only buy between 1 and 4 tickets")]
        public int Quantity { get; set; } = 1;
        public int ExistingTicketsCount { get; set; }
        public int StockAvailable { get; set; }
    }

    public class MyTicketsViewModel
    {
        public List<GlobalFests.EFModels.Ticket> Tickets { get; set; } = new();

        public Dictionary<int, string> QrCodes { get; set; } = new();
    }


    //  WORLD MAP VIEW MODEL

    public class WorldMapViewModel
    {
        public EventsSearchModel? Search { get; set; }

        public CursorResult<EventWorldMapDto> Events { get; set; }

        [ValidateNever]
        public List<Country>? Countries { get; set; }
        [ValidateNever]
        public List<EventType>? EventTypes { get; set; }

        [ValidateNever]
        public List<Genre>? Genres { get; set; }
    }

    // for user profile 

    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; } 

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 chars")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }

        public int? CountryId { get; set; }

        
        public string RoleName { get; set; } = "";
        public string CountryName { get; set; } = "";
        public bool IsVerified { get; set; }

       
        public SelectList? Countries { get; set; }
    }

}
