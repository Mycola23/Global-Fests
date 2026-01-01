namespace GlobalFests.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string CountryName { get; set; } = "Unknown";
        public bool Verified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
