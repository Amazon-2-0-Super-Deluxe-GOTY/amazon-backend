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
        [Comment("Main prodct reference")]
        public Guid? ProductId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ShortDescription { get; set; }
        public double Price { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageUrl { get; set; }
        public double? DiscountPrice { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Brand { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        [InverseProperty("Product")]
        public List<ProductImage>? productImages { get; set; }
        [InverseProperty("Product")]
        public List<ProductProperty>? pProps { get; set; }
        public List<AboutProductItem>? AboutProductItems { get; set; }
        public List<Product>? Products { get; set; }
        public List<ProductRate>? ProductRates { get; set; }
    }
}
