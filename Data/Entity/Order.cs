using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Order
    {
        [Key]
        [Column(TypeName = "varchar(255)")]
        public string Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string orderKey { get; set; }    // Unique value for "Order" to sort "Product"
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string UserId { get; set; }
        public User? User { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
