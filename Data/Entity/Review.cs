using amazon_backend.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Review
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Mark { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User? User { get; set; }
    public Product? Product { get; set; }
    public List<ReviewImage>? ReviewImages { get; set; }
    public List<ReviewTag>? ReviewTags { get; set; }
    public List<ReviewLike>? ReviewLikes { get; set; }
}
