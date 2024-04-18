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
       
        public UsersController(IUserDao userDao, IHashService hashService, DataContext dataContext, 
            IKdfService kdfService, 
            IEmailService emailService, 
            IRandomService randomService, 
            IServiceProvider services, 
            IConfiguration configuration)
        {
            _hashService = hashService;
            _dataContext = dataContext;
            _kdfService = kdfService;
            _emailService = emailService;
            _randomService = randomService;
            _services = services;
            
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

        [HttpPost]
        public User CreateUser()
        {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                Email = "example@example.com",
                Password = "aabbcc",
                PasswordSalt = "111",
                Role = "client"
            };

            userDao.Add(user);
            return user;
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


        [HttpPost("registration/{email}/{password}/{repeat_password}")]
        public async Task<IActionResult> Register(string email, string password, string repeat_password)
        {
            #region validation
            if (password != repeat_password)
            {
                return BadRequest("Passwords not same");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return BadRequest("User is created");
            }
            #endregion
            String salt = _randomService.RandomString(16);
            if (ValidatePassword(password) && ValidateEmail(email)) 
            {
            
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = _kdfService.GetDerivedKey(password, salt),
                    Role = "User",
                    Password = password,
                    PasswordSalt = salt,
                };

                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest("Password or email invalid");
            }
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
    }



}

