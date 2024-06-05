using System.ComponentModel.DataAnnotations;

namespace amazon_backend.Data.Model
{
    public class CategoryPropertyKeyModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsFilter { get; set; }
        public bool IsRequired { get; set; }
        [Required]
        public string NameCategory { get; set; }
        public bool IsDeleted { get; set; }
        public uint CategoryId { get; set; }
    }
}
