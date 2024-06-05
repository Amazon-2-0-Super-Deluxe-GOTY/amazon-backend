using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace amazon_backend.Data.Model
{
    public class СategoryModel
    {
        public string? ParentCategory { get; set; }
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Image { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public bool IsVisible { get; set; }
        public uint Logo { set; get; }

    }
}
