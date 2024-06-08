namespace amazon_backend.DTO
{
    public class CategoryPropertyKeyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFilter { get; set; }
        public bool IsRequired { get; set; }
    }
}
