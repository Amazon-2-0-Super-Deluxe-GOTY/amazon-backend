using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [ApiController]
    [Route("seller-profile")]
    public class SellerProfileController : Controller
    {
        ISellerProfileDao sellerProfileDao;

        public SellerProfileController(ISellerProfileDao sellerProfileDao)
        {
            this.sellerProfileDao = sellerProfileDao;
        }

        [HttpGet]
        public SellerProfile[] GetSellerProfiles()
        {
            return sellerProfileDao.GetAll();
        }

        [HttpGet]
        [Route("seller/{id}")]
        public Results<NotFound, Ok<SellerProfile>> GetSellerProfile(Guid id)
        {
            SellerProfile? profile = sellerProfileDao.GetById(id);

            if (profile is null) return TypedResults.NotFound();

            return TypedResults.Ok(profile);
        }

        [HttpPost]
        public SellerProfile CreateSellerProfile()
        {
            SellerProfile profile = new SellerProfile()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test",
                OrganizationId = Guid.NewGuid(),
                OrganizationName = "Test",
                InterestRate = 0,
            };

            sellerProfileDao.Add(profile);
            return profile;
        }

        [HttpPut]
        [Route("{id}")]
        public Results<NotFound, Ok<SellerProfile>> UpdateSellerProfile(Guid id)
        {
            SellerProfile? profile = sellerProfileDao.GetById(id);

            if (profile is null) return TypedResults.NotFound();

            profile.FirstName = "Test2";
            sellerProfileDao.Update(profile);

            return TypedResults.Ok(profile);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteSellerProfile(Guid id)
        {
            sellerProfileDao.Delete(id);
            return Ok();
        }
    }
}
