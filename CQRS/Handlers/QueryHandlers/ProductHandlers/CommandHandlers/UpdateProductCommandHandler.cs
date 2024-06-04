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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, Result<ProductViewProfile>>
    {
        private readonly TokenService _tokenService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ISlugHelper _slugHelper;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly IRandomService _randomService;

        public UpdateProductCommandHandler(TokenService tokenService, DataContext dataContext, IMapper mapper, ISlugHelper slugHelper, ILogger<UpdateProductCommandHandler> logger, IRandomService randomService)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
            _mapper = mapper;
            _slugHelper = slugHelper;
            _logger = logger;
            _randomService = randomService;
        }

        public async Task<Result<ProductViewProfile>> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
        {
            var decodeResult = await _tokenService.DecodeTokenFromHeaders(true);
            if (!decodeResult.isSuccess)
            {
                return new() { isSuccess = decodeResult.isSuccess, message = decodeResult.message, statusCode = decodeResult.statusCode };
            }
            try
            {
                Product? product = await _dataContext.Products
                    .Include(p => p.AboutProductItems)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductProperties)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.productId));
                if (product == null)
                {
                    return new("Product not found") { statusCode = 404 };
                }
                if (request.name != null)
                {
                    product.Name = request.name;
                    product.Slug = _slugHelper.GenerateSlug(request.name) + $"-{_randomService.ConfirmCode(4)}";
                }
                if (request.price.HasValue)
                {
                    product.Price = request.price.Value;
                }
                if (request.discount.HasValue)
                {
                    product.DiscountPercent = request.discount;
                }
                if (request.code != null)
                {
                    product.Code = request.code;
                }
                if (request.quantity.HasValue)
                {
                    product.Quantity = request.quantity.Value;
                }
                if (request.categoryId.HasValue)
                {
                    Category? category = await _dataContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.categoryId);
                    if (category == null)
                    {
                        return new("Category not found") { statusCode = 404 };
                    }
                    product.CategoryId = category.Id;
                }
                if (request.productDetails != null && request.productDetails.Count != 0)
                {
                    Category? category = await _dataContext.Categories
                        .AsNoTracking()
                        .Include(c => c.CategoryPropertyKeys)
                        .FirstOrDefaultAsync(c => c.Id == request.categoryId);

                    if (product.ProductProperties != null && product.ProductProperties.Count != 0)
                    {
                        List<ProductProperty>? existProps = null;
                        if (category?.CategoryPropertyKeys != null && category.CategoryPropertyKeys.Count != 0)
                        {
                            var categoryKeys = category.CategoryPropertyKeys.Select(ck => ck.Name.ToLower()).ToList();
                            existProps = product.ProductProperties.Where(pp => categoryKeys.Contains(pp.Key.ToLower())).ToList();
                        }
                        if (existProps != null)
                        {
                            product.ProductProperties.Clear();
                            product.ProductProperties = existProps;
                            foreach (var item in request.productDetails)
                            {
                                var prop = product.ProductProperties.FirstOrDefault(pp => pp.Key.ToLower() == item.name.ToLower());
                                if (prop != null)
                                {
                                    if (prop.Value.ToLower() != item.text.ToLower())
                                    {
                                        product.ProductProperties.Remove(prop);
                                        ProductProperty? pProp = await _dataContext.ProductProperties
                                            .FirstOrDefaultAsync(pp => pp.Key.ToLower() == item.name.ToLower() && pp.Value.ToLower() == item.text.ToLower());
                                        if (pProp != null)
                                        {
                                            product.ProductProperties.Add(pProp);
                                        }
                                        else
                                        {
                                            var newpProp = new ProductProperty()
                                            {
                                                Id = Guid.NewGuid(),
                                                Key = item.name,
                                                Value = item.text,
                                                IsOption = false
                                            };
                                            await _dataContext.AddAsync(newpProp);
                                            await _dataContext.SaveChangesAsync();
                                            product.ProductProperties.Add(newpProp);
                                        }
                                    }
                                }
                                else
                                {
                                    ProductProperty? pProp = await _dataContext.ProductProperties
                                        .FirstOrDefaultAsync(pp => pp.Key.ToLower() == item.name.ToLower() && pp.Value.ToLower() == item.text.ToLower());
                                    if (pProp != null)
                                    {
                                        product.ProductProperties.Add(pProp);
                                    }
                                    else
                                    {
                                        var newpProp = new ProductProperty()
                                        {
                                            Id = Guid.NewGuid(),
                                            Key = item.name,
                                            Value = item.text,
                                            IsOption = false
                                        };
                                        await _dataContext.AddAsync(newpProp);
                                        await _dataContext.SaveChangesAsync();
                                        product.ProductProperties.Add(newpProp);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var categoryPropKeys = category?.CategoryPropertyKeys;
                        if (categoryPropKeys != null && categoryPropKeys.Count != 0)
                        {
                            var propKeys = request.productDetails.Select(pd => pd.name).ToList();
                            var condResult = categoryPropKeys.All(k => propKeys.Contains(k.Name));
                            if (!condResult)
                            {
                                return new("The product does not contain all the required features") { statusCode = 400 };
                            }
                        }
                        product.ProductProperties = new();
                        foreach (var item in request.productDetails)
                        {
                            ProductProperty? pProp = await _dataContext.ProductProperties.AsNoTracking()
                                .FirstOrDefaultAsync(pp => pp.Key.ToLower() == item.name.ToLower() && pp.Value.ToLower() == item.text.ToLower());
                            if (pProp != null)
                            {
                                product.ProductProperties.Add(pProp);
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
                                product.ProductProperties.Add(pProp);
                            }
                        }
                    }
                }

                if (request.images != null)
                {
                    if (product.ProductImages == null)
                    {
                        product.ProductImages = new();
                    }
                    if (request.images.Count != 0)
                    {
                        if (product.ProductImages.Count == 10)
                        {
                            return new("Maximum 10 product images") { statusCode = 400 };
                        }
                        var canAddCount = (10 - product.ProductImages.Count());
                        if (canAddCount > 0)
                        {
                            var count = 0;
                            foreach (var item in request.images)
                            {
                                ProductImage? image = await _dataContext.ProductImages.FirstOrDefaultAsync(pi => pi.Id == Guid.Parse(item));
                                if (image != null)
                                {
                                    if (!product.ProductImages.Contains(image))
                                    {
                                        product.ProductImages.Add(image);
                                    }
                                }
                                count++;
                                if (canAddCount == count) break;
                            }
                        }
                        if (product.ProductImages.Count != 0)
                        {
                            product.ImageUrl = product.ProductImages[0].ImageUrl;
                        }
                    }
                }
                _dataContext.Update(product);
                await _dataContext.SaveChangesAsync();
                if (request.aboutProduct != null)
                {
                    if (product.AboutProductItems != null && product.AboutProductItems.Count != 0)
                    {
                        for (int i = product.AboutProductItems.Count - 1; i >= 0; i--)
                        {
                            var item = product.AboutProductItems[i];
                            _dataContext.Remove(item);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                    if (request.aboutProduct.Count != 0)
                    {
                        foreach (var item in request.aboutProduct)
                        {
                            AboutProductItem aboutProduct = new()
                            {
                                Id = Guid.NewGuid(),
                                ProductId = product.Id,
                                Title = item.name,
                                Text = item.text
                            };
                            await _dataContext.AddAsync(aboutProduct);
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                }

                return new("Ok") { statusCode = 200, data = _mapper.Map<ProductViewProfile>(product) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new("See server logs") { statusCode = 500 };
            }
        }
    }
}
