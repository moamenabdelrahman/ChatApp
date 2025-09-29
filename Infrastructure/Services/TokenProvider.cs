using Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenProvider
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenProvider(IConfiguration config)
        {
            this._config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
        }

        public string CreateToken(AppUser appUser)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, appUser.UserName)
                ]),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler();

            var token = handler.CreateToken(tokenDescriptor);
            
            return token;
        }
    }
}
