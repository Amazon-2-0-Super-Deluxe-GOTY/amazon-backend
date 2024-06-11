using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace amazon_backend.Data.Entity
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? FirstName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? LastName { get; set; }
        [Column(TypeName = "varchar(2083)")]
        public string? AvatarUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string? PhoneNumber { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? TempEmail { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string PasswordSalt { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string PasswordHash { get; set; }
        [Required]
        [Column(TypeName = "varchar(128)")]
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public string? EmailCode { get; set; }
        
        public List<Review>? Reviews { get; set; }
        public List<ReviewImage>? ReviewImages { get; set; }
        public List<TokenJournal>? TokenJournals { get; set; }
        public List<ReviewLike>? ReviewLikes { get; set; }
        public List<Product>? WishedProducts { get; set; }
    }
}
