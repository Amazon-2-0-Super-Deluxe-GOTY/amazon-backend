using amazon_backend.Data.Dao;

namespace amazon_backend.DTO
{
    public class CategoryDto
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisible { get; set; }
        public uint Logo { get; set; }
        public List<CategoryPropertyKeyDto>? CategoryPropertyKeys { get; set; }
    }
}
