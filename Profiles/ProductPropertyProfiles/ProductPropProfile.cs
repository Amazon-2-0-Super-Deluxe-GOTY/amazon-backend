using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Profiles.ProductPropertyProfiles
{
    public class ProductPropProfile
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
