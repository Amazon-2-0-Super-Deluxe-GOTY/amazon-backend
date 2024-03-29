﻿using System;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
namespace amazon_backend.Data.Dao
{
    public interface ICartDao
    {
        Cart GetCart(int id);
        void AddToCart(int cartId, Product product);
        void RemoveFromCart(int cartId, Product product);
        void UpdateCart(Cart cart);
        List<Product> GetCartContents(int userId);
    }


    public class CartDao : ICartDao
    {
        private readonly DataContext _context;

        public CartDao(DataContext context)
        {
            _context = context;
        }

        public Cart GetCart(int id)
        {
            return _context.Carts.Include(c => c.UserId).FirstOrDefault(c => c.Id == id);
            //return null;
        }

        public void AddToCart(int cartId, Product product)
        {
            var cart = GetCart(cartId);
            //cart..Add(product);
            _context.SaveChanges();
        }

        public void RemoveFromCart(int cartId, Product product)
        {
            //var cart = GetCart(cartId);
            //cart.Products.Remove(product);
            //_context.SaveChanges();
        }

        public void UpdateCart(Cart cart)
        {
            _context.Carts.Update(cart);
            _context.SaveChanges();
        }

        public List<Product> GetCartContents(int userId)
        {
            //var cart = _context.Carts.Include(c => c.Products).FirstOrDefault(c => c.UserId == userId);
            //return cart?.Products;
            return null;
        }
    }
}
