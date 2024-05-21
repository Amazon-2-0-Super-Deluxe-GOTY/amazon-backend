using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ReviewImage
    {
        public Guid Id { get; set; }
        [Column(TypeName ="varchar(255)")]
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public Review? Review { get; set; }
    }
}
