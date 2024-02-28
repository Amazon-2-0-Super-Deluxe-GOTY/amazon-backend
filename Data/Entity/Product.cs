using System.ComponentModel.DataAnnotations;

namespace amazon_backend.Data.Entity
{
    public class Product
    {
        [Key]
        public string Id { get; set; }
    }
}
