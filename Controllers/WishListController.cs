using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("wishlists")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly WishListDao wishListDao;
        public WishListController(WishListDao wishListDao)
        {
            this.wishListDao = wishListDao;
        }
        [HttpGet]
        public WishList[] GetWishLists()
        {
            return wishListDao.GetAll();
        }
        [HttpPost]
        public WishList CreateWishList()
        {
            var wishList = new WishList
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "NewWishList",
                CreatedAt = DateTime.Now
            };
            wishListDao.Add(wishList);
            return wishList;
        }
        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<WishList>> GetWishListById(string id)
        {
            Guid wishListId;
            try
            {
                wishListId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            WishList? wishList = wishListDao.GetById(wishListId);
            if (wishList is not null) return TypedResults.Ok(wishList);
            return TypedResults.NotFound();
        }
        [HttpPut]
        [Route("/restore-wishlist/{id}")]
        public IActionResult RestoreWishList(string id)
        {
            Guid wishListId;
            try
            {
                wishListId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            wishListDao.Restore(wishListId);
            return Ok();
        }
        [HttpDelete]
        [Route("/delete-wishlist/{id}")]
        public IActionResult DeleteWishList(string id)
        {
            Guid wishListId;
            try
            {
                wishListId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            wishListDao.Delete(wishListId);
            return Ok();
        }
    }
}