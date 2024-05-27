using amazon_backend.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using amazon_backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using amazon_backend.Profiles.JwtTokenProfiles;
namespace amazon_backend.Services.JWTService
{

    public class TokenService
    {
        private readonly string _secretKey;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public TokenService(IOptions<Options.Token.TokenOptions> options, DataContext dataContext, IMapper mapper)
        {
            options.Value.Validate();
            _secretKey = options.Value.SecretKey;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<JwtTokenProfile?> GetTokenByUserId(Guid userId)
        {
            User? user = await _dataContext
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);
            if (user == null)
            {
                return null;
            }
            await DeleteTokensIfExpired(userId);
            TokenJournal? tokenJournal = await _dataContext
                .TokenJournals
                .Include(tj => tj.Token)
                .AsNoTracking()
                .FirstOrDefaultAsync(tj => tj.UserId == userId && tj.DeactivatedAt == null);
            if (tokenJournal != null)
            {
                if (tokenJournal.Token != null)
                {
                    string token = tokenJournal.Token.Token;
                    var result = ValidateToken(token);
                    if (result != null)
                    {
                        return _mapper.Map<JwtTokenProfile>(tokenJournal.Token);
                    }
                    _dataContext.JwtTokens.Remove(tokenJournal.Token);
                    await _dataContext.SaveChangesAsync();
                }
                _dataContext.Remove(tokenJournal);
                await _dataContext.SaveChangesAsync();
            }
            var newToken = await GenerateToken(userId);
            return _mapper.Map<JwtTokenProfile>(newToken);
        }

        private async Task<bool> DeleteTokensIfExpired(Guid userId)
        {
            List<TokenJournal>? tokenJournal = await _dataContext
                .TokenJournals
                .Include(tj => tj.Token)
                .Where(tj => tj.UserId == userId && tj.DeactivatedAt != null)
                .ToListAsync();
            if (tokenJournal != null)
            {
                foreach (var tj in tokenJournal)
                {
                    if (tj.Token != null)
                    {
                        if (DateTime.Now > tj.Token.ExpirationDate)
                        {
                            _dataContext.JwtTokens.Remove(tj.Token);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private async Task<JwtToken> GenerateToken(Guid userId)
        {
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString())
            });
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var exp = DateTime.UtcNow.AddHours(24);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = exp,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var newToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenModel = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = tokenHandler.WriteToken(newToken),
                ExpirationDate = exp
            };
            var tokenJournal = new TokenJournal
            {
                Id = Guid.NewGuid(),
                TokenId = tokenModel.Id,
                UserId = userId,
                ActivatedAt = DateTime.Now
            };
            await _dataContext.AddAsync(tokenModel);
            await _dataContext.AddAsync(tokenJournal);
            await _dataContext.SaveChangesAsync();
            return tokenModel;
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
                ClockSkew = TimeSpan.Zero
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

        public async Task<Guid?> DecodeTokenFromHeaders(HttpContext httpContext)
        {
            string? token = httpContext.Request.Headers["Authorization"];
            if (token != null)
            {
                Guid? userId = await DecodeToken(token);
                if (userId != null)
                {
                    return userId;
                }
            }
            return null;
        }

        public async Task<Guid?> DecodeToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            var index = token.IndexOf(" ");
            if (index < 0)
            {
                return null;
            }
            var _token = token.Substring(index + 1);
            JwtToken? jwtToken = await _dataContext.JwtTokens.AsNoTracking().FirstOrDefaultAsync(j => j.Token == _token);
            if(jwtToken == null)
            {
                return null;
            }
            TokenJournal? tj = await _dataContext.TokenJournals.AsNoTracking().FirstOrDefaultAsync(t => t.TokenId == jwtToken.Id);
            if (tj == null)
            {
                return null;
            }
            if (tj.DeactivatedAt != null)
            {
                return null;
            }
            return tj.UserId;
        }

    }
}
