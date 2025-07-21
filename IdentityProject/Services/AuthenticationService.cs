using AutoMapper;
using IdentityProject.Abstracts;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

            //user.Role = "User";
            user.CreatedAt = Convert.ToDateTime(DateTime.Now.ToShortTimeString());

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userDto.Roles);
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

            //user.Role = "Admin";
            user.CreatedAt = Convert.ToDateTime(DateTime.Now.ToShortTimeString());

            var result = await _userManager.CreateAsync(user, adminDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, adminDto.Roles);
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

            bool result = (_user is not null && await _userManager.CheckPasswordAsync(_user, userDto.Password));  // await _userManager.CheckPasswordAsync, kullanıcının şifresini kontrol eder.
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

        public async Task<TokenDTO> CreateJwtTokenAsync(bool populateExpire)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken(); // Kullanıcının refresh token'ını oluşturur.

            _user.RefreshToken = refreshToken; // Kullanıcının refresh token'ını ayarlar.

            if (populateExpire == true) // Eğer populateExpire true ise, kullanıcının refresh token'ının son kullanma tarihini ayarlar.
            {
                _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token'ın son kullanma tarihini 7 gün ileriye ayarlar.
            }
            else
            {
                _user.RefreshTokenExpiryTime = DateTime.UtcNow; // Refresh token'ın son kullanma tarihini şu anki zamana ayarlar.
            }

            await _userManager.UpdateAsync(_user); // Kullanıcıyı günceller.

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions); // JWT token'ı oluşturur ve string olarak döndürür. 

            return new TokenDTO
            {
                AccessToken = accessToken, // Erişim token'ını ayarlar. // Erişim token'ı, kullanıcının kimliğini doğrulamak için kullanılır.
                RefreshToken = refreshToken // Refresh token'ı ayarlar. // Refresh token, kullanıcının oturumunu yenilemek için kullanılır.
            };
        }

        public async Task<TokenDTO> CreateRefreshTokenAsync(TokenDTO tokenDTO)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDTO.AccessToken); // Süresi dolmuş token'dan ClaimsPrincipal nesnesini alır.
            var user = await _userManager.FindByNameAsync(principal.Identity.Name); // ClaimsPrincipal'dan kullanıcı adını alır ve kullanıcıyı bulur.

            if (user is null || user.RefreshToken != tokenDTO.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) 
            {
                throw new SecurityTokenException("Invalid token or user not found"); // Eğer kullanıcı bulunamazsa veya refresh token geçersizse, bir SecurityTokenException fırlatılır.
            }

            _user = user; // Kullanıcıyı günceller.

            return await CreateJwtTokenAsync(false); // Yeni bir JWT token oluşturur ve döndürür. // false parametresi, refresh token'ın son kullanma tarihini doldurmaması için kullanılır.
        }

        #region Helper Methods

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // JWT ayarlarını alır. // JwtSettings, appsettings.json dosyasındaki JwtSettings bölümünü temsil eder.
            var secretKey = jwtSettings["SecretKey"]; // SecretKey, JWT'nin imzalanması için kullanılan simetrik bir güvenlik anahtarıdır.

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // Sağlayıcının doğrulanmasını sağlar.
                ValidateAudience = true, // Hedef kitlenin doğrulanmasını sağlar.
                ValidIssuer = jwtSettings["ValidIssuer"], // Geçerli verici (issuer) ayarlanır.
                ValidAudience = jwtSettings["ValidAudience"], // Geçerli hedef kitle (audience) ayarlanır.
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // JWT'nin imzalanması için kullanılan simetrik güvenlik anahtarı ayarlanır.
                ValidateIssuerSigningKey = true, // İmza anahtarının doğrulanmasını sağlar.
                ValidateLifetime = false, // Token'ın ömrünün doğrulanmasını devre dışı bırakır. Bu, süresi dolmuş token'ların da işlenebilmesini sağlar.
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // JWT token'larını işlemek için JwtSecurityTokenHandler kullanılır.
            SecurityToken securityToken;

            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken); // Token'ı doğrular ve ClaimsPrincipal nesnesini döndürür. // out securityToken, doğrulanan token'ı döndürür.
            if (claimsPrincipal == null) // Eğer ClaimsPrincipal nesnesi null ise, bir SecurityTokenException fırlatılır.
            {
                throw new SecurityTokenException("Invalid token"); // Eğer token geçerli değilse, bir SecurityTokenException fırlatılır.
            }

            var jwtSecurityToken = securityToken as JwtSecurityToken; // Doğrulanan token'ı JwtSecurityToken olarak alır. // Burada bir cast işlemi yapılır. // Eğer token JwtSecurityToken ise, jwtSecurityToken değişkenine atanır.
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) // Eğer token JwtSecurityToken değilse veya HmacSha256 algoritması ile imzalanmamışsa, null döndürür.
            {
                throw new SecurityTokenException("Invalid token"); // Eğer token geçerli değilse, bir SecurityTokenException fırlatılır.
            }
            if (jwtSecurityToken.ValidTo < DateTime.UtcNow) // Eğer token'ın geçerlilik süresi geçmişse, ClaimsPrincipal nesnesini döndürür.
            {
                throw new SecurityTokenException("Token has expired"); // Eğer token süresi dolmuşsa, bir SecurityTokenException fırlatılır.
            }



            return claimsPrincipal; // Doğrulanan ClaimsPrincipal nesnesini döndürür.
        }

        private String GenerateRefreshToken() // Kullanıcının refresh token'ını oluşturur.
        {
            var randomNumber = new byte[32]; // 32 byte uzunluğunda bir dizi oluşturur.

            using (var randomNumberGenerator = RandomNumberGenerator.Create()) // RandomNumberGenerator, rastgele sayılar üretmek için kullanılır.
            {
                randomNumberGenerator.GetBytes(randomNumber); // Rastgele sayıları diziye doldurur.
            }

            return Convert.ToBase64String(randomNumber); // Dizi'yi Base64 string'e dönüştürür ve döndürür.
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // JWT ayarlarını alır. // JwtSettings, appsettings.json dosyasındaki JwtSettings bölümünü temsil eder.
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"], // JWT'nin vericisi (issuer) ayarlanır.
                audience: jwtSettings["ValidAudience"], // JWT'nin hedef kitlesi (audience) ayarlanır.
                claims: claims, // JWT'ye eklenecek claim'ler ayarlanır.
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])), // JWT'nin geçerlilik süresi ayarlanır.
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
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]); // SecretKey, JWT'nin imzalanması için kullanılan simetrik bir güvenlik anahtarıdır. // Encoding.UTF8.GetBytes, string'i byte dizisine dönüştürür.
            var secretKey = new SymmetricSecurityKey(key); // SecretKey, JWT'nin imzalanması için kullanılan simetrik bir güvenlik anahtarıdır.

            return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256); // HmacSha256, JWT'nin imzalanması için kullanılan algoritmadır.
        }

        #endregion Helper Methods
    }
}