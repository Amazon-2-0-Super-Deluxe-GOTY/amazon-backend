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
        public string UserId { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
