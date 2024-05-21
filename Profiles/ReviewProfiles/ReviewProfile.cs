using amazon_backend.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Profiles.ReviewProfiles
{
    public class ReviewProfile
    {
        public Guid Id { get; set; }
        public int Mark { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
        public List<ReviewImageProfile>? ReviewImages { get; set; }
        public List<ReviewTagProfile>? ReviewTags { get; set; }
    }
}
