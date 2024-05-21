using amazon_backend.Data.Entity;
using amazon_backend.Data;
using amazon_backend.Services.JWTService;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
            ILogger<SessionAuthMiddleware> logger,
            DataContext dataContext, TokenService tokenService)
        {
            String? userId = context.Session.GetString("authUserId");
            String? token = context.Session.GetString("userToken");
            if (userId is not null && token is not null)
            {
                try
                {
                    User? authUser = await dataContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId.ToString());
                    if (authUser is not null)
                    {
                        var journal = await dataContext.TokenJournals.Include(p => p.Token).FirstOrDefaultAsync(j => j.Id == authUser.Id && j.IsActive == true);
                        if (journal == null)
                        {
                            throw new Exception("Journal not found");
                        }
                        if (journal.Token.ExpirationDate <= DateTime.UtcNow)
                        {
                            //journal.DeactivatedAt= DateTime.UtcNow;
                            //journal.IsActive = false;
                            context.Request.Method = "POST";
                            context.Request.Headers["X-Original-Method"] = "GET";
                            //context.Response.Redirect("/logout");
                            return;
                        }
                        context.Items.Add("authUser", authUser);

                        Claim[] claims = new Claim[]
                        {
                            new Claim(ClaimTypes.Sid ,authUser.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier ,authUser.Email),
                        };
                        var principals = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                claims,
                                nameof(SessionAuthMiddleware)));

                        context.User = principals;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "SessionMiddleware");
                }



            }

            await _next(context);
        }
    }
    public static class SessionAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionAuthMiddleware>();
        }
    }

}
