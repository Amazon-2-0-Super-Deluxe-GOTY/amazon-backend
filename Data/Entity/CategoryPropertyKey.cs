using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class CategoryPropertyKey
    {
        public Guid Id { get; set; }
       
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        public bool IsFilter { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDeleted { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
