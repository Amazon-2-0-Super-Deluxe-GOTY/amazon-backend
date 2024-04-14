using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Profiles.ProductProfiles
{
    public class ProductColorProfile
    {
        public string HashColor { get; set; }
        public string Name { get; set; }
        public string AlbumReference { get; set; }
    }
}
