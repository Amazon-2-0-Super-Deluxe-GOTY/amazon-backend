using amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests;
using amazon_backend.CQRS.Commands.CategoryRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryPropertyQueryHandlers
{
    public class UpdateCategoryPropertyQueryHandlers : IRequestHandler<UpdateCategoryPropertyKeyCommandRequst, Result<CategoryPropertyKeyProfile>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;

        public UpdateCategoryPropertyQueryHandlers(IMapper mapper, DataContext dataContext, TokenService tokenService)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<Result<CategoryPropertyKeyProfile>> Handle(UpdateCategoryPropertyKeyCommandRequst request, CancellationToken cancellationToken)
        {

            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;


            if(request.Name == null || request.NameCategory == null)
            {
                return new("No parameters for update") { statusCode = 400 };
            }

            CategoryPropertyKey? categoryPropertyKey = await _dataContext.CategoryPropertyKeys
                .Include(x => x.Name)
                .Include(x => x.Id)
                .Include(x => x.Name)
                .Include(x => x.CategoryId)
                .Include(x => x.IsRequired)
                .Include(x => x.IsFilter)
                .Include(x => x.IsDeleted)
                .AsSplitQuery()
                .Where(r => r.Name == request.Name).FirstOrDefaultAsync();

            if (categoryPropertyKey != null)
            {
                categoryPropertyKey.NameCategory = request.NameCategory;
                categoryPropertyKey.IsFilter = request.IsFilter;
                categoryPropertyKey.IsRequired = request.IsRequired;
                categoryPropertyKey.IsDeleted = request.IsDeleted;
                _dataContext.CategoryPropertyKeys.Update(categoryPropertyKey);
            }

            return new("Review not found") { statusCode = 404 };
        }
    }
}
