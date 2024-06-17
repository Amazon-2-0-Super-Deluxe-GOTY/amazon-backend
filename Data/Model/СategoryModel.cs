using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace amazon_backend.Data.Model
{
    public class СategoryModel
    {
        public uint? ParentCategoryId {  get; set; }
        public string? ParentCategory { get; set; }
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Image { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string Logo { set; get; }

    }
}
