﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Password { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string PasswordSalt { get; set; }
        [Required]
        [Column(TypeName = "varchar(128)")]
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }

        public ClientProfile? ClientProfile { get; set; }
    }
}
