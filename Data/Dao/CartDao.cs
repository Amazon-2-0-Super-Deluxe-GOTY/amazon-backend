using System;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
namespace amazon_backend.Data.Dao
{
    public interface ICartDao : IDataAccessObject<Cart, Guid>
    {
        CartItemDao[] GetItemsInCart(Guid cartId);
        void AddItemToCart(Guid cartId, Product product, int quantity);
        void RemoveItemFromCart(Guid cartId, Guid productId);
        void UpdateItemQuantity(Guid cartId, Guid productId, int quantity);
    }

    public class CartDao : ICartDao
    {
        private readonly DataContext _context;
        public CartDao(DataContext context)
        {
            _context = context;
        }

        public void Add(Cart cart)
        {
            if (cart != null)
            {
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            Cart? cart = _context.Carts.Find(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }
        }

        public Cart[] GetAll()
        {
            return _context.Carts.ToArray();
        }

        public Cart? GetById(Guid id)
        {
            return _context.Carts.Find(id);
        }

        public void Update(Cart cart)
        {
            if (cart != null)
            {
                _context.Carts.Update(cart);
                _context.SaveChanges();
            }
        }

        public CartItemDao[] GetItemsInCart(Guid cartId)
        {
            return _context.CartItems.Where(ci => ci.CartId == cartId).ToArray();
        }

        public void AddItemToCart(Guid cartId, Product product, int quantity)
        {
            var cartItem = new CartItemDao { CartId = cartId, Product = product, Quantity = quantity };
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();
        }

        public void RemoveItemFromCart(Guid cartId, Guid productId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.Product.Id == productId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public void UpdateItemQuantity(Guid cartId, Guid productId, int quantity)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.Product.Id == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }
        }
    }

}
