using amazon_backend.Profiles.UserProfiles;

namespace amazon_backend.Profiles.ReviewProfiles
{
    public class ReviewProfile
    {
        public Guid Id { get; set; }
        public int Mark { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public int Likes { get; set; }
        public bool CurrentUserLiked { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReviewUserProfile? User { get; set; }
        public List<ReviewImageProfile>? ReviewImages { get; set; }
        public List<ReviewTagProfile>? ReviewTags { get; set; }
    }
}
