using System.ComponentModel.DataAnnotations;

namespace IdentityProject.DTOs.IdentityDTOs
{
    public record UserDTOForRegistration
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        [Required(ErrorMessage = "Username is required!")]
        public string UserName { get; init; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        //public ICollection<string> Roles { get; init; } = new List<string> { "User" }; // Default role for new users
    }
}