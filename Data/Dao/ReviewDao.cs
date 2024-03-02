/*using System;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

public interface IReviewDao
{
    Review GetReview(int id);
    void AddReview(Review review);
    void RemoveReview(int reviewId);
    void UpdateReview(Review review);
    List<Review> GetReviewsByUser(int userId);
    List<Review> GetReviewsByProduct(int productId);
}


public class ReviewDao : IReviewDao
{
    private readonly DataContext _context;

    public ReviewDao(DataContext context)
    {
        _context = context;
    }

    public Review GetReview(int id)
    {
        return _context.Reviews.Include(r => r._User).Include(r => r._Product).FirstOrDefault(r => r.Id == id);
    }

    public void AddReview(Review review)
    {
        _context.Reviews.Add(review);
        _context.SaveChanges();
    }

    public void RemoveReview(int reviewId)
    {
        var review = GetReview(reviewId);
        _context.Reviews.Remove(review);
        _context.SaveChanges();
    }

    public void UpdateReview(Review review)
    {
        _context.Reviews.Update(review);
        _context.SaveChanges();
    }

    public List<Review> GetReviewsByUser(int userId)
    {
        return _context.Reviews.Include(r => r._User).Include(r => r._Product).Where(r => r.UserId == userId).ToList();
    }

    public List<Review> GetReviewsByProduct(int productId)
    {
        return _context.Reviews.Include(r => r._User).Include(r => r._Product).Where(r => r.ProductId == productId).ToList();
    }
}
*/