using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("wishlistitems")]
    [ApiController]
    public class WishListItemController : ControllerBase
    {
        private readonly WishListItemDao wishListItemDao;
        public WishListItemController(WishListItemDao wishListItemDao)
        {
            this.wishListItemDao = wishListItemDao;
        }
        [HttpGet]
        public WishListItem[] GetWishListItems()
        {
            return wishListItemDao.GetAll();
        }
        [HttpPost]
        public WishListItem CreateWishListItem()
        {
            var wishListItem = new WishListItem
            {
                Id = Guid.NewGuid(),
                WishListId = Guid.NewGuid(),
                ProductId = Guid.NewGuid()
            };
            wishListItemDao.Add(wishListItem);
            return wishListItem;
        }
        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<WishListItem>> GetWishListItemById(string id)
        {
            Guid wishListItemId;
            try
            {
                wishListItemId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            WishListItem? wishListItem = wishListItemDao.GetById(wishListItemId);
            if (wishListItem is not null) return TypedResults.Ok(wishListItem);
            return TypedResults.NotFound();
        }
        [HttpPut]
        [Route("/restore-wishlistitem/{id}")]
        public IActionResult RestoreWishListItem(string id)
        {
            Guid wishListItemId;
            try
            {
                wishListItemId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            wishListItemDao.Restore(wishListItemId);
            return Ok();
        }
        [HttpDelete]
        [Route("/delete-wishlistitem/{id}")]
        public IActionResult DeleteWishListItem(string id)
        {
            Guid wishListItemId;
            try
            {
                wishListItemId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            wishListItemDao.Delete(wishListItemId);
            return Ok();
        }
    }
}