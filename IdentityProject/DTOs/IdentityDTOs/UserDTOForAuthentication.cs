using System.ComponentModel.DataAnnotations;

namespace IdentityProject.DTOs.IdentityDTOs
{
    public record UserDTOForAuthentication
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; init; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; init; }
    }
}