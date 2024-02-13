namespace amazon_backend.Data.Model
{
    public class CreateCategoryModel
    {
        public uint? ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Image { get; set; }
    }
}
