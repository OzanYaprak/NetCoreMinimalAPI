using IdentityProject.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Abstracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserDTOForRegistration userDto); // Kullanıcı kaydı için asenkron metot
    }
}