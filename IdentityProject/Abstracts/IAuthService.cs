using IdentityProject.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Abstracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserDTOForRegistration userDto); // Kullanıcı kaydı için asenkron metot
        Task<IdentityResult> RegisterAdminAsync(AdminDTOForRegistration adminDto); // Admin kaydı için asenkron metot
        Task<bool> ValidateUserCredentialsAsync(UserDTOForAuthentication userDto); // Kullanıcı kiml7ik bilgilerini doğrulamak için metot
        Task<TokenDTO> CreateJwtTokenAsync(bool populateExpire); // JWT token oluşturmak için metot // populateExpire parametresi, token'ın son kullanma tarihini doldurup doldurmayacağını belirler
        Task<TokenDTO> CreateRefreshTokenAsync(TokenDTO tokenDTO); // Refresh token oluşturmak için metot
    }
}