using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid orderKey { get; set; }    // Unique value for "Order" to sort "Product"
        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        [Required]
        public Guid ProductId { get; set; }
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