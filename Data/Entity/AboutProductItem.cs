using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class AboutProductItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public Product? Product { get; set; }
    }
}
