using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ReviewImage
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Column(TypeName ="varchar(255)")]
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public List<Review>? Review { get; set; }
        public User? User { get; set; }
    }
}
