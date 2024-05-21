using amazon_backend.CQRS.Commands.RewiewTagRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ReviewTagQueryHandlers
{
    public class CreateReviewTagCommandHandler : IRequestHandler<CreateReviewTagCommandRequest, Result<Guid>>
    {
        private DataContext _dataContext;
        private ILogger<CreateReviewTagCommandHandler> _logger;
        public CreateReviewTagCommandHandler(DataContext dataContext, ILogger<CreateReviewTagCommandHandler> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(CreateReviewTagCommandRequest request, CancellationToken cancellationToken)
        {
            ReviewTag newTag = new()
            {
                Id = Guid.NewGuid(),
                Name = request.name,
                Description = request.description,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await _dataContext.ReviewTags.AddAsync(newTag, cancellationToken);
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs");
            }
            return new(newTag.Id);
        }
    }
}
