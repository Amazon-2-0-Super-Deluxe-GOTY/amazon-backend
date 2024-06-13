namespace amazon_backend.Data.Entity
{
    public class CategoryImage
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public List<Category>? Categories { get; set; }
        public int CategoryId {  get; set; }
    }
}
