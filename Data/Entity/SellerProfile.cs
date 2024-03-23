using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace amazon_backend.Data.Entity
{
    public class SellerProfile
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string OrganizationName { get; set; }
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
        public int InterestRate { get; set; }
    }
}
