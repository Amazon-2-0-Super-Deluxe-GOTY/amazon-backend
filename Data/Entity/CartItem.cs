﻿namespace amazon_backend.Data.Entity
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public Product? Product { get; set; }
        public Cart? Cart { get; set; }
    }
}
