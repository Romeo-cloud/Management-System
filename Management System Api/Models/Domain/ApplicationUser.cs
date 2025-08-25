using Microsoft.AspNetCore.Identity;
namespace Management_System_Api.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAtUtc { get; set; }

    }
}
