using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class Category
    {
        [Key]
        public uint Id { get; set; }
        public uint? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
