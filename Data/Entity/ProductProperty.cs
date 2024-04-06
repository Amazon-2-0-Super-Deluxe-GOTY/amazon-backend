using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ProductProperty
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName ="varchar(255)")]
        public string Key { get; set; }
        [Column(TypeName ="varchar(255)")]
        public string Value { get; set; }
        public bool IsOption { get; set; }
        // NavigationProperty
        [ForeignKey("Id")]
        public Product Product { get; set; }
    }
}
