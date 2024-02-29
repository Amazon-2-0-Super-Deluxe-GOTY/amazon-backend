using System.ComponentModel;
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
        public WishList? WishList { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
