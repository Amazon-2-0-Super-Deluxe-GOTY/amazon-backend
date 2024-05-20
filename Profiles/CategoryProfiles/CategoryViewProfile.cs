namespace amazon_backend.Profiles.CategoryProfiles
{
    public class CategoryViewProfile
    {
        public uint Id { get; set; }
        public uint ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
