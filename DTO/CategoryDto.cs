using amazon_backend.Data.Dao;

namespace amazon_backend.DTO
{
    public class CategoryDto
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public string Logo { get; set; }
        public uint? ParentId {  get; set; }
        public List<CategoryPropertyKeyDto>? CategoryPropertyKeys { get; set; }
    }
}
