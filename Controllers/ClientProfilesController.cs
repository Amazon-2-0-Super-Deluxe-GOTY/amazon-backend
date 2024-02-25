using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("client-profile")]
    public class ClientProfilesController : ControllerBase
    {
        IClientProfileDao clientProfileDao;

        public ClientProfilesController(IClientProfileDao clientProfileDao)
        {
            this.clientProfileDao = clientProfileDao;
        }

        [HttpGet]
        public ClientProfile[] GetClientProfiles()
        {
            return clientProfileDao.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<ClientProfile>> GetClientProfile(Guid id)
        {
            ClientProfile? profile = clientProfileDao.GetById(id);

            if (profile is null) return TypedResults.NotFound();

            return TypedResults.Ok(profile);
        }

        [HttpPost]
        public ClientProfile CreateClientProfile()
        {
            ClientProfile profile = new ClientProfile()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("76953395-44a6-49a7-aa9e-6e61fc56805b"),
                FirstName = "Test",
                LastName = "Test",
            };

            clientProfileDao.Add(profile);
            return profile;
        }

        [HttpPut]
        [Route("{id}")]
        public Results<NotFound, Ok<ClientProfile>> UpdateClientProfile(Guid id)
        {
            ClientProfile? profile = clientProfileDao.GetById(id);

            if (profile is null) return TypedResults.NotFound();

            profile.FirstName = "Test2";
            clientProfileDao.Update(profile);

            return TypedResults.Ok(profile);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteClientProfile(Guid id)
        {
            clientProfileDao.Delete(id);
            return Ok();
        }
    }
}
