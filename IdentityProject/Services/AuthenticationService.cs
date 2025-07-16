using AutoMapper;
using IdentityProject.Abstracts;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityProject.Services
{
    public class AuthenticationService : IAuthService
    {
        #region Constructor

        private User _user;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthenticationService(UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
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

        public async Task<string> CreateJwtTokenAsync()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // JWT ayarlarını alır. // JwtSettings, appsettings.json dosyasındaki JwtSettings bölümünü temsil eder.
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings.GetValue<string>("ValidIssuer"), // JWT'nin vericisi (issuer) ayarlanır.
                audience: jwtSettings.GetValue<string>("ValidAudience"), // JWT'nin hedef kitlesi (audience) ayarlanır.
                claims: claims, // JWT'ye eklenecek claim'ler ayarlanır.
                expires: DateTime.Now.AddMinutes(jwtSettings.GetValue<int>("ExpirationInMinutes")), // JWT'nin geçerlilik süresi ayarlanır.
                signingCredentials: signingCredentials // JWT'nin imzalanması için gerekli imzalama bilgileri ayarlanır.
            );
            return tokenOptions; // Oluşturulan JWT token'ı döndürülür.
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName), // Kullanıcının adını ClaimTypes.Name ile ekler.
            };

            var roles = await _userManager.GetRolesAsync(_user); // Kullanıcının rollerini alır.

            foreach (var role in roles) // Kullanıcının rollerini döngü ile ekler.
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Her rolü ClaimTypes.Role ile ekler.
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // JWT ayarlarını alır. // JwtSettings, appsettings.json dosyasındaki JwtSettings bölümünü temsil eder.
            var key = Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("SecretKey")); // SecretKey, JWT'nin imzalanması için kullanılan simetrik bir güvenlik anahtarıdır. // Encoding.UTF8.GetBytes, string'i byte dizisine dönüştürür.
            var secretKey = new SymmetricSecurityKey(key); // SecretKey, JWT'nin imzalanması için kullanılan simetrik bir güvenlik anahtarıdır.

            return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256); // HmacSha256, JWT'nin imzalanması için kullanılan algoritmadır.
        }
    }
}