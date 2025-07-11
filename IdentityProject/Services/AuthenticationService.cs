using AutoMapper;
using IdentityProject.Abstracts;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Services
{
    public class AuthenticationService : IAuthService
    {
        #region Constructor

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
            var user = _mapper.Map<User>(userDto);
            user.Role = "User";

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, new string[] { "User" });
            }

            return result;
        }
    }
}