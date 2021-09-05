using Core.Entities.Identity;
using Core.Interfaces.AppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AppServices
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        // Symmetric security key as a type of encryption where only one a secret key which we're 
        // going to store on our server is used to both encrypt and decrypt our signature in the token.
        // It's essential that this never leaves our server, otherwise anybody's going to be able to impersonate any user on our system.
        private readonly SymmetricSecurityKey _key;
        public TokenService(
            IConfiguration config, 
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtAuthentication:Key"]));
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            // to add the role in Web token to use id with Authorize
            IdentityOptions _option = new IdentityOptions();
            // each user is going to have a list of their claims inside this JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(_option.ClaimsIdentity.RoleClaimType, role)
            };

            //
            var Credential = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = Credential,
                Issuer = _config["JwtAuthentication:Issuer"]

            };

            // generate a Token with all tokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
