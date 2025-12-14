using System.ComponentModel.DataAnnotations;

namespace GlobalFests.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
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

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class EventSearchViewModel
    {
        public string? Title { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? TypeId { get; set; }

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
}
