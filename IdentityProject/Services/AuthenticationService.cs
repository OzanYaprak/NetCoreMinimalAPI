using AutoMapper;
using IdentityProject.Abstracts;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Services
{
    public class AuthenticationService : IAuthService
    {
        #region Constructor

        private User _user;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AuthenticationService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        #endregion Constructor

        public async Task<IdentityResult> RegisterUserAsync(UserDTOForRegistration userDto)
        {
            Validate(userDto); // Admin DTO'sunu doğrular.
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            {
                throw new ValidationException("Username and Password cannot be null or empty.");
            }

            var user = _mapper.Map<User>(userDto);
            
            user.Role = "User";
            user.CreatedAt = Convert.ToDateTime(DateTime.Now.ToShortTimeString());

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, new string[] { "User" });
            }

            return result;
        }


        public async Task<IdentityResult> RegisterAdminAsync(AdminDTOForRegistration adminDto)
        {
            Validate(adminDto); // Admin DTO'sunu doğrular.
            if (string.IsNullOrEmpty(adminDto.UserName) || string.IsNullOrEmpty(adminDto.Password))
            {
                throw new ValidationException("Username and Password cannot be null or empty.");
            }

            var user = _mapper.Map<User>(adminDto);
            
            user.Role = "Admin";
            user.CreatedAt = Convert.ToDateTime(DateTime.Now.ToShortTimeString());

            var result = await _userManager.CreateAsync(user, adminDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, new string[] { "Admin" });
            }
            return result;
        }

        public async Task<bool> ValidateUserCredentialsAsync(UserDTOForAuthentication userDto)
        {
            Validate(userDto); // Kullanıcı DTO'sunu doğrular.
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            {
                throw new ValidationException("Username and Password cannot be null or empty.");
            }

            // Kullanıcı adı ve şifre boş değilse, kullanıcıyı bulmaya çalışır.
            _user = await _userManager.FindByNameAsync(userDto.Username);

            bool result = (_user is not null && await _userManager.CheckPasswordAsync(_user, userDto.Password)); // Kullanıcı bulunursa ve şifre doğruysa, result true olur. // _user is not null, kullanıcı bulunmuşsa true döner. // await _userManager.CheckPasswordAsync, kullanıcının şifresini kontrol eder.
            if (result)
            {
                _user.LastLoginDate = DateTime.Now; // Kullanıcı giriş yaptıktan sonra son giriş tarihini günceller.
                await _userManager.UpdateAsync(_user);  
            }
            return result;
        }

        private void Validate<T>(T item)
        {
            var validationResults = new List<ValidationResult>(); // ValidationResult, doğrulama sonuçlarını tutar.
            var context = new ValidationContext(item); // ValidationContext, doğrulama bağlamını tutar.
            var isValid = Validator.TryValidateObject(item, context, validationResults, true); // Validator, doğrulama işlemini yapar. // TryValidateObject, doğrulama işlemini yapar ve sonuçları validationResults listesine ekler. // true parametresi, tüm özelliklerin doğrulanmasını sağlar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(select => select.ErrorMessage)); // Doğrulama hatalarını birleştirir.
                throw new ValidationException(errors); // Doğrulama hatalarını içeren bir ValidationException fırlatır.
            }
        }
    }
}