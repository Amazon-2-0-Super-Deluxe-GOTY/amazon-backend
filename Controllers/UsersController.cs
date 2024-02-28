using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserDao userDao;

        public UsersController(IUserDao userDao)
        {
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
    }
}
