using amazon_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Slug { get; set; }
        public uint CategoryId { get; set; }
        public Guid? ProductId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Code { get; set; }
        public double Price { get; set; }
        public int? DiscountPercent { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductProperty>? ProductProperties { get; set; }
        public List<AboutProductItem>? AboutProductItems { get; set; }
        public List<Product>? Products { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
