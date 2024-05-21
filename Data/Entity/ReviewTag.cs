using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ReviewTag
    {
        public Guid Id { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        [Column(TypeName ="varchar(255)")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
