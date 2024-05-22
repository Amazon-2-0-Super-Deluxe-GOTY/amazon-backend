using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using amazon_backend.Options.Token;
namespace amazon_backend.Services.JWTService
{

    public class TokenService
    {
        private readonly string _secretKey;
        private readonly string _secretApiKey;
        public TokenService(IOptions<Options.Token.TokenOptions> options)
        {
            options.Value.Validate();
            _secretKey = options.Value.SecretKey;
            _secretApiKey = "chJh+LOfz601Ny3sWJHlS0UwrL5cDzh7sClIepfCkw4=";
        }
        public Token GenerateToken(Guid userId, out TokenJournal tokenJournal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var claims = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            //new Claim(ClaimTypes.Role, user.Role.ToString())
        });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
               
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenModel = new Token
            {
                Id = Guid.NewGuid(),
                _Token = tokenHandler.WriteToken(token),
                ExpirationDate = tokenDescriptor.Expires ?? DateTime.UtcNow.AddHours(3)
            };

            tokenJournal = new TokenJournal
            {
                Id = Guid.NewGuid(),
                TokenId = tokenModel.Id,
                UserId = userId,
                IsActive = true,
                ActivatedAt = DateTime.Now
            };

            return tokenModel;
        }
        public string GenerateApiToken(User user, bool rememberMe)
        {
            var symmetricKey = Convert.FromBase64String(_secretApiKey); // Используйте достаточно длинный ключ
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            var claims = new ClaimsIdentity(new[]{
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            });


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = rememberMe ? DateTime.UtcNow.AddYears(10) : DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                var claims = jwtToken.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

                var header = jwtToken.Header.Select(h => new
                {
                    h.Key,
                    h.Value
                });
                var userIdClaim = claims?.FirstOrDefault(c => c.Type == "nameid")?.Value;

                return new
                {
                    Claims = claims,
                    id = userIdClaim
                };
            }

            throw new ArgumentException("Invalid token");
        }
    }
   
}
