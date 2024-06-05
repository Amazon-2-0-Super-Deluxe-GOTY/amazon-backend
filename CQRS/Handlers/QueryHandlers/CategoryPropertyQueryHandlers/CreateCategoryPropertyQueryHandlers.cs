using amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests;
using amazon_backend.Models;
using amazon_backend.Data.Entity;
using amazon_backend.Data;
using MediatR;
using amazon_backend.Data.Model;
using amazon_backend.Services.JWTService;
using AutoMapper;
using amazon_backend.Data.Dao;
using Microsoft.EntityFrameworkCore;
using amazon_backend.Profiles.ReviewProfiles;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryPropertyQueryHandlers
{
    public class CreateCategoryPropertyQueryHandlers : IRequestHandler<CreateCategoryPropertyKeyCommandRequst, Result<CategoryPropertyKeyProfile>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly ILogger<CreateCategoryPropertyQueryHandlers> _logger;
        private readonly IMapper _mapper;
        private readonly CategoryPropertyKeyDao _categoryPropertyKeyDao;

        public CreateCategoryPropertyQueryHandlers(DataContext dataContext, TokenService tokenService, ILogger<CreateCategoryPropertyQueryHandlers> logger, IMapper mapper, CategoryPropertyKeyDao categoryPropertyKeyDao)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
            _categoryPropertyKeyDao = categoryPropertyKeyDao;
        }

        public async Task<Result<CategoryPropertyKeyProfile>> Handle(CreateCategoryPropertyKeyCommandRequst request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;

            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Name == request.NameCategory);
            if (category == null)
            {
                return new("category not found") { statusCode = 404 };
            }
            var categoryPropkey = request.Id;
            CategoryPropertyKey propertyKey = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsFilter = request.IsFilter,
                IsRequired = request.IsRequired,
                IsDeleted = request.IsDeleted,
                NameCategory = request.NameCategory,
                CategoryId = category.Id
            };
            
            
            try
            {
                await _dataContext.CategoryPropertyKeys.AddAsync(propertyKey, cancellationToken);
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs");
            }


            
            return new("Created") { data = _mapper.Map<CategoryPropertyKeyProfile>(propertyKey), statusCode = 201 };
        }
    }
    
}
