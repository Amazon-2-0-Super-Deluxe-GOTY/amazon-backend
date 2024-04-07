using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Mvc;
namespace amazon_backend.Controllers
{

    [ApiController]
    [Route("controller")]
    public class CartController : ControllerBase
    {
        private readonly ICartDao _cartDao;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartDao cartDao, ILogger<CartController> logger)
        {
            _cartDao = cartDao;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Cart> GetCarts()
        {
            return _cartDao.GetAll();
        }

        [HttpGet("{id}")]
        public Cart GetCart(Guid id)
        {
            return _cartDao.GetById(id);
        }

        [HttpPost]
        public void AddCart(Cart cart)
        {
            _cartDao.Add(cart);
        }

        [HttpPut("{id}")]
        public void UpdateCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                throw new ArgumentException("Cart ID mismatch");
            }
            _cartDao.Update(cart);
        }

        [HttpDelete("{id}")]
        public void DeleteCart(Guid id)
        {
            _cartDao.Delete(id);
        }

        [HttpGet("{id}/items")]
        public CartItemDao[] GetItemsInCart(Guid id)
        {
            return _cartDao.GetItemsInCart(id);
        }

        [HttpPost("{id}/items")]
        public void AddItemToCart(Guid id, Product product, int quantity)
        {
            _cartDao.AddItemToCart(id, product, quantity);
        }

        [HttpDelete("{cartId}/items/{productId}")]
        public void RemoveItemFromCart(Guid cartId, Guid productId)
        {
            _cartDao.RemoveItemFromCart(cartId, productId);
        }

        [HttpPut("{cartId}/items/{productId}")]
        public void UpdateItemQuantity(Guid cartId, Guid productId, int quantity)
        {
            _cartDao.UpdateItemQuantity(cartId, productId, quantity);
        }
    }
}
