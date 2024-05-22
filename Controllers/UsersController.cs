using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Services.Email;
using amazon_backend.Services.Hash;
using amazon_backend.Services.KDF;
using amazon_backend.Services.Random;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using amazon_backend.Services.JWTService;
using AutoMapper.Execution;


namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserDao userDao;
        private readonly IHashService _hashService;
        private readonly DataContext _dataContext;
        private readonly IKdfService _kdfService;
        private readonly IEmailService _emailService;
        private readonly IRandomService _randomService;
        private readonly IServiceProvider _services;
        private readonly TokenService _tokenService;
        
        public UsersController(IUserDao userDao, IHashService hashService, DataContext dataContext, 
            IKdfService kdfService, 
            IEmailService emailService, 
            IRandomService randomService, 
            IServiceProvider services, 
            IConfiguration configuration,
            TokenService tokenService,
            IEmailService _emailService1)
        {
            _hashService = hashService;
            _dataContext = dataContext;
            _kdfService = kdfService;
            _emailService = emailService;
            _randomService = randomService;
            _services = services;
            _tokenService = tokenService;
            this.userDao = userDao;
            
        }

        [HttpGet]
        public User[] GetUsers()
        {
            return userDao.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<User>> GetUser(Guid id)
        {
            User? user = userDao.GetById(id);

            if (user is null) return TypedResults.NotFound();

            return TypedResults.Ok(user);
        }

        

        [HttpPut]
        [Route("{id}")]
        public Results<NotFound, Ok<User>> UpdateUser(Guid id)
        {
            User? user = userDao.GetById(id);

            if (user is null) return TypedResults.NotFound();

            user.Email = "example2@example.com";
            userDao.Update(user);

            return TypedResults.Ok(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            userDao.Delete(id);
            return Ok();
        }

        [HttpPut]
        [Route("{id}/restore")]
        public IActionResult RestoreUser(Guid id)
        {
            userDao.Restore(id);
            return Ok();
        }

        #region Registration


        [HttpPost("registration/")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            string confirmEmailCode = _randomService.ConfirmCode(6);
            #region validation

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == registerModel.Email);
            //if (existingUser != null)
            //{
            //    return BadRequest("User is created");
            //}
            #endregion
            String salt = _randomService.RandomString(16);
            if (ValidatePassword(registerModel.Password) && ValidateEmail(registerModel.Email)) 
            {
                
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Email = registerModel.Email,
                    PasswordHash = _kdfService.GetDerivedKey(registerModel.Password, salt),
                    Role = "User",
                    PasswordSalt = salt,
                    EmailCode = confirmEmailCode,
                };
               
                _emailService.SendEmail(registerModel.Email, "test body", "test message");

                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest("Password or email invalid");
            }
        }



        private EmailConfirmToken _GenerateEmailConfirmToken(Data.Entity.User user)
        {
            Data.Entity.EmailConfirmToken emailConfirmToken = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UserEmail = user.Email,
                Moment = DateTime.Now,
                Used = 0
            };
            _dataContext.EmailConfirmTokens.Add(emailConfirmToken);
            return emailConfirmToken;
        }


     

        public static bool ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

           
            if (password.Length < 8)
            {
                return false;
            }

            
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

         
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return false;
            }

            return true;
        }

        #endregion


        #region login


        [HttpPost("login/")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var hashedPassword = _kdfService.GetDerivedKey(loginModel.Password, user.PasswordSalt);
            if (hashedPassword != user.PasswordHash)
            {
                return Unauthorized(new { message = "Invalid password" });
            }

            var tokenJournal = user.TokenJournals?.FirstOrDefault(tj => tj.IsActive);
            var token = tokenJournal?.Token?._Token;

            if (token == null)
            {
                var new_token = _tokenService.GenerateToken(user.Id, out tokenJournal);
                _dataContext.Tokens.Add(new_token);
                _dataContext.TokenJournals.Add(tokenJournal);
                _dataContext.SaveChanges();
                HttpContext.Session.SetString("userToken", new_token._Token);
            }
            else
            {
                HttpContext.Session.SetString("userToken", token);
            }

            if (user.Id != null)
            {

                HttpContext.Session.SetString("authUserId", user.Id.ToString());
                Response.Cookies.Append("SessionId", HttpContext.Session.Id);
               
            }
            string? userToken = HttpContext.Session.GetString("userToken");
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                createdAt = user.CreatedAt,
                emailCode = user.EmailCode,
                token = userToken,
                Firsname = user.FirstName,
                Lastname = user.LastName,
                AvatarUrl = user.AvatarUrl,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber

            });
        }
        #endregion

        [HttpGet("isAuthenticated/")]
        public IActionResult IsAuthenticated()
        {

            string? userId = HttpContext.Session.GetString("userToken");
            if (userId != null)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    token = userId
                });
            }

            return Ok(new { isAuthenticated = false });
        }


        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Remove("authUserId");
                string? token = HttpContext.Session.GetString("userToken");
                if (token == null)
                {
                    HttpContext.Session.Clear();
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        Response.Cookies.Delete(cookie);
                    }
                    return Ok();

                }
                var journal = await _dataContext.TokenJournals.Include(p => p.Token).FirstOrDefaultAsync(j => j.Token._Token == token && j.IsActive == true);
                if (journal != null)
                {
                    journal.DeactivatedAt = DateTime.UtcNow;
                    journal.IsActive = false;
                    await _dataContext.SaveChangesAsync();
                }
                HttpContext.Session.Clear();
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }



}

