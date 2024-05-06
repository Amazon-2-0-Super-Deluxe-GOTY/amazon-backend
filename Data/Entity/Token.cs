using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace amazon_backend.Data.Entity
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("token_id")]
        public Guid Id { get; set; }

        [Column("token")]
        public string _Token { get; set; }


        [Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [JsonIgnore]
        public List<TokenJournal>? TokenJournals { get; set; }
    }
}
