using System;
using System.Threading;
using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;

public interface IReviewDao : IDataAccessObject<Review, Guid>
{
    public Task<bool> AddAsync(Review item);
    public Task<bool> DeleteAsync(Guid id);
    public Task<bool> DeleteReviewImageAsync(Guid imageId);
    public Task<List<Review>> GetAllAsync();
    public Task<Review?> GetByIdAsync(Guid id);
    public Task<bool> UpdateAsync(Review item);
}


public class ReviewDao : IReviewDao
{
    private readonly DataContext _context;
    private readonly ILogger<ReviewDao> _logger;

    public ReviewDao(DataContext context, ILogger<ReviewDao> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void Add(Review item)
    {
        if (item != null)
        {
            try
            {
                _context.Reviews.Add(item);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    public async Task<bool> AddAsync(Review item)
    {
        if (item != null)
        {
            try
            {
                await _context.Reviews.AddAsync(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        return false;
    }

    public void Delete(Guid id)
    {
        try
        {
            Review? item = _context.Reviews.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            Review? item = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public Review[] GetAll()
    {
        try
        {
            if (_context.Reviews != null && _context.Reviews.Count() != 0)
            {
                return _context.Reviews.AsNoTracking().ToArray();
            }
            return null;
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<List<Review>> GetAllAsync()
    {
        try
        {
            if (_context.Reviews != null && _context.Reviews.Count() != 0)
            {
                return await _context.Reviews.AsNoTracking().ToListAsync();
            }
            return null;
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public Review? GetById(Guid id)
    {
        try
        {
            Review? item = _context.Reviews.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                return item;
            }
            return null;
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public void Update(Review item)
    {
        if (item != null)
        {
            try
            {
                Review? review = _context.Reviews.FirstOrDefault(x => x.Id == item.Id);
                if (review != null)
                {
                    review = item;
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        try
        {
            Review? item = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.ReviewImages)
                .Include(r => r.ReviewTags)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                return item;
            }
            return null;
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Review item)
    {
        if (item != null)
        {
            try
            {
                Review? review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (review != null)
                {
                    review = item;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        return false;
    }

    public async Task<bool> DeleteReviewImageAsync(Guid imageId)
    {
        ReviewImage? image = await _context.ReviewImages.FirstOrDefaultAsync(i => i.Id == imageId);
        if (image == null)
        {
            return false;
        }
        _context.Remove(image);
        await _context.SaveChangesAsync();
        return true;
    }
}
