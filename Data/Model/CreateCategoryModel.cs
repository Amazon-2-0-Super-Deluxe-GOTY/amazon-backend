namespace amazon_backend.Data.Model
{
    public class CreateCategoryModel
    {
        public uint? ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImageId { get; set; }
        public string? ParentCategoryName { get; set; }
        public uint Logo { set; get; }
        public List<CategoryPropertyKeyModel> PropertyKeys { get; set; }
    }
}
