using amazon_backend.CQRS.Commands.ProductCommands;
using amazon_backend.Data;
using amazon_backend.Data.Entity;
using amazon_backend.Models;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.Random;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace amazon_backend.CQRS.Handlers.QueryHandlers.ProductHandlers.CommandHandlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommandRequest, Result<ProductViewProfile>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ISlugHelper _slugHelper;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly IRandomService _randomService;

        public CreateProductCommandHandler(TokenService tokenService, DataContext dataContext, IMapper mapper, ISlugHelper slugHelper, ILogger<CreateProductCommandHandler> logger, IRandomService randomService)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
            _mapper = mapper;
            _slugHelper = slugHelper;
            _logger = logger;
            _randomService = randomService;
        }

        public async Task<Result<ProductViewProfile>> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            try
            {
                Category? category = await _dataContext.Categories.AsNoTracking().Include(c => c.CategoryPropertyKeys).FirstOrDefaultAsync(c => c.Id == request.categoryId);
                if (category == null)
                {
                    return new("Category not found") { statusCode = 404 };
                }

                var categoryPropKeys = category.CategoryPropertyKeys;
                if (categoryPropKeys != null && categoryPropKeys.Count != 0)
                {
                    var propKeys = request.productDetails.Select(pd => pd.name).ToList();
                    var condResult = categoryPropKeys.All(k => propKeys.Contains(k.Name));
                    if (!condResult)
                    {
                        return new("The product does not contain all the required features") { statusCode = 400 };
                    }
                }
                Product newProduct = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Name = request.name,
                    Slug = _slugHelper.GenerateSlug(request.name) + $"-{_randomService.ConfirmCode(4)}",
                    Price = request.price,
                    DiscountPercent = request.discount,
                    CategoryId = category.Id,
                    Quantity = request.quantity,
                    Code = request.code
                };
                await _dataContext.AddAsync(newProduct);
                await _dataContext.SaveChangesAsync();
                newProduct.ProductImages = new();
                foreach (var item in request.images)
                {
                    ProductImage? image = await _dataContext
                        .ProductImages.FirstOrDefaultAsync(pi => pi.Id == Guid.Parse(item));
                    if (image != null)
                    {
                        newProduct.ProductImages.Add(image);
                    }
                }
                newProduct.ImageUrl = newProduct.ProductImages[0].ImageUrl;
                newProduct.ProductProperties = new();
                foreach (var item in request.productDetails)
                {
                    ProductProperty? pProp = await _dataContext.ProductProperties
                        .FirstOrDefaultAsync(pp => pp.Key.ToLower() == item.name.ToLower() && pp.Value.ToLower() == item.text.ToLower());
                    if (pProp != null)
                    {
                        newProduct.ProductProperties.Add(pProp);
                    }
                    else
                    {
                        pProp = new()
                        {
                            Id = Guid.NewGuid(),
                            Key = item.name,
                            Value = item.text,
                            IsOption = false
                        };
                        await _dataContext.AddAsync(pProp);
                        await _dataContext.SaveChangesAsync();
                        newProduct.ProductProperties.Add(pProp);
                    }
                }
                _dataContext.Update(newProduct);
                await _dataContext.SaveChangesAsync();
                if (request.aboutProduct != null && request.aboutProduct.Count != 0)
                {
                    foreach (var item in request.aboutProduct)
                    {
                        AboutProductItem aboutProduct = new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = newProduct.Id,
                            Title = item.name,
                            Text = item.text
                        };
                        await _dataContext.AddAsync(aboutProduct);
                        await _dataContext.SaveChangesAsync();
                    }
                }
                return new("Created") { statusCode = 201, data = _mapper.Map<ProductViewProfile>(newProduct) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
        }
    }
}
