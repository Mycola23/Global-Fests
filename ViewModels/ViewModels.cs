using System.ComponentModel.DataAnnotations;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
    }

    public class RegisterViewModel
    {
        public RegisterUserModel User { get; set; } = new RegisterUserModel();

        [ValidateNever]
        public List<Country>? Countries { get; set; }
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
    }

    public class EventsViewModel
    {
        public EventsSearchModel? Search { get; set; }

        public CursorResult<EventDto> Events { get; set; }

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
    }

    public class EventDetailsViewModel
    {
        public EventWithDetailsDto Event { get; set; } = new EventWithDetailsDto();
        
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
}
