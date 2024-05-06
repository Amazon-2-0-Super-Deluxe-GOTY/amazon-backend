using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace amazon_backend.Data.Entity
{
    public class TokenJournal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("token_journal_id")]
        public Guid Id { get; set; }

        [Column("token_id")]
        public Guid TokenId { get; set; }

        [JsonIgnore]
        public Token? Token { get; set; }


        [Column("user_id")]
        public Guid? UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("activated_at")]
        public DateTime ActivatedAt { get; set; }


        [Column("deactivated_at")]
        public DateTime DeactivatedAt { get; set; }
    }
}
