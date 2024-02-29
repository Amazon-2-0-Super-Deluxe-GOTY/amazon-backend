using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        public double Price { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageUrl { get; set; }
        public double? DiscountPrice { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Brand { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [InverseProperty("Product")]
        public List<ProductImages> productImages { get; set; }
    }
}
