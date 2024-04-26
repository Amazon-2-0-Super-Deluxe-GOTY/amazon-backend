using System.Text.Json.Serialization;

namespace amazon_backend.Data.Model
{
    public class ProductFilterBody
    {
        public Dictionary<string, string[]> filterProperties { get; set; }
        public string categoryName { get; set; }
        public double maxPrice { get; set; }
        public double minPrice { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
}
