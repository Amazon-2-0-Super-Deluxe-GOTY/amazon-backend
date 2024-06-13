using amazon_backend.CQRS.Queries.Request.OrderRequests;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.OrderProfiles;
using amazon_backend.Services.JWTService;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.OrderHandlers.QueryHandlers
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQueryRequest, Result<List<OrderProfile>>>
    {
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public GetOrdersQueryHandler(DataContext dataContext, TokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<List<OrderProfile>>> Handle(GetOrdersQueryRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders();

            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }

            var user = decodeResult.data;
            var orderQuery = _dataContext
                    .Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .Include(o => o.DeliveryAddresses)
                    .AsSplitQuery().AsQueryable();
            List<Order> allOrders = null;

            if (user.Role != "Admin")
            {
                orderQuery = orderQuery.Where(u => u.UserId == user.Id);
            }

            allOrders = await orderQuery
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip(request.pageSize * (request.pageIndex - 1))
                    .Take(request.pageSize)
                    .ToListAsync();

            if (allOrders != null && allOrders.Count != 0)
            {
                var orderProfiles = _mapper.Map<List<OrderProfile>>(allOrders);
                int totalCount = await orderQuery.CountAsync();
                int pagesCount = (int)Math.Ceiling(totalCount / (double)request.pageSize);
                return new(orderProfiles, pagesCount) { statusCode = 200, message = "Ok" };
            }
            return new("Orders not found") { statusCode = 404 };
        }
    }
}
