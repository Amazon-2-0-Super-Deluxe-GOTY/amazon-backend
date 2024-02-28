using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Product
    {
        [Key]
        [Column(TypeName = "varchar(255)")]
        public string Id { get; set; }
    }
}
