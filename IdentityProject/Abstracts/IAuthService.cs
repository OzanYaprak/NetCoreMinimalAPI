using IdentityProject.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Abstracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserDTOForRegistration userDto); // Kullanıcı kaydı için asenkron metot
        Task<IdentityResult> RegisterAdminAsync(AdminDTOForRegistration adminDto); // Admin kaydı için asenkron metot
        Task<bool> ValidateUserCredentialsAsync(UserDTOForAuthentication userDto); // Kullanıcı kimlik bilgilerini doğrulamak için metot
    }
}