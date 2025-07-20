using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
        public DateTime LastLoginDate { get; set; } = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
    }
}