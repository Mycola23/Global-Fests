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

    public class OrganizerPanelViewModel
    {
        public CursorResult<PerformerDto>? Performers { get; set; }

        public CursorResult<EventOrganizerDto>? Events { get; set; }
    }
}
