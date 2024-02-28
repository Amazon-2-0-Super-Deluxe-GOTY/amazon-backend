using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class ClientProfile
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        //public User User { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar(2083)")]
        public string? AvatarUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? PhoneNumber { get; set; }
    }
}
