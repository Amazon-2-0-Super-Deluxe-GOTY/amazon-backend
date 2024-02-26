using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class WishListItem
    {
        [Key]
        [Column(TypeName = "varchar(255)")]
        public string Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string WishListId { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string ProductId { get; set; }
    }
}
