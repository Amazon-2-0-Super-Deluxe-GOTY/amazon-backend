﻿using System.ComponentModel.DataAnnotations.Schema;

namespace amazon_backend.Data.Entity
{
    public class CategoryPropertyKey
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
        bool IsFilter { get; set; }
        bool IsRequired { get; set; }
        bool IsDeleted { get; set; }
        public List<Category>? Categories { get; set; }
    }
}