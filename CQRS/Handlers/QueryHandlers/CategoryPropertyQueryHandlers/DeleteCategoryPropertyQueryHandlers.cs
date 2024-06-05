using amazon_backend.CQRS.Commands.CategoryPropertyKeyRequests;
using amazon_backend.CQRS.Commands.ReviewRequests;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Services.JWTService;
using MediatR;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.CategoryPropertyQueryHandlers
{
    public class DeleteCategoryPropertyQueryHandlers : IRequestHandler<DeleteCategoryPropertyKeyCommandRequst, Result<Guid>>
    {
        
        private readonly TokenService _tokenService;

        public DeleteCategoryPropertyQueryHandlers(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

     

        public async Task<Result<Guid>> Handle(DeleteCategoryPropertyKeyCommandRequst request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            User user = decodeResult.data;
            var propKeyName = request.CategoryPropertyKeyName;

            // Dao  Propkey CategoryPropertyKey categoryPropertyKey = await 
            return new("Not found") { statusCode = 404 };
        }
    }
}
