using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ProductColor
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string HashColor {  get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string AlbumReference { get; set; }
        public bool IsDeleted { get; set; }
        public Product? product { get; set; }
    }
}
