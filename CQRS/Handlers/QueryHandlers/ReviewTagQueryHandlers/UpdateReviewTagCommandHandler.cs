using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class UpdateReviewTagCommandHandler : IRequestHandler<UpdateReviewTagCommandRequest, Result<Guid>>
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UpdateReviewTagCommandHandler> _logger;
        public UpdateReviewTagCommandHandler(DataContext dataContext, ILogger<UpdateReviewTagCommandHandler> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(UpdateReviewTagCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ReviewTag? reviewTag = await _dataContext.ReviewTags.Where(rt => rt.Id == Guid.Parse(request.reviewTagId)).FirstOrDefaultAsync();
                if(reviewTag != null) 
                {
                    reviewTag.Name = request.name;
                    reviewTag.Description = request.description;
                    await _dataContext.SaveChangesAsync();
                    return new(reviewTag.Id);
                }
                return new("Not Found");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs");
            }
        }
    }
}
