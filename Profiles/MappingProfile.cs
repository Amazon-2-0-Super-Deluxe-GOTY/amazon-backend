using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Models;

namespace amazon_backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductCardProfile>()
                .ForMember(dest => dest.GeneralRate, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductRates != null && src.ProductRates.Count != 0)
                        {
                            return src.ProductRates.Sum(pr => pr.Mark) / src.ProductRates.Count;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.ReviewsCount, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductRates != null && src.ProductRates.Count != 0)
                        {
                            return src.ProductRates.Count;
                        }
                        return 0;
                    });
                })
                 .ForMember(dest => dest.DiscountPercentage, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         if (src.DiscountPrice.HasValue && src.Price != 0)
                         {
                             return (src.Price - src.DiscountPrice.Value) / src.Price * 100;
                         }
                         else
                         {
                             return 0;
                         }
                     });
                 });
            CreateMap<ProductImage, ProductImageProfile>();
            CreateMap<Category, CategoryViewProfile>();
            CreateMap<Product, ProductViewProfile>()
                .ForMember(dest => dest.DiscountPercentage, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.DiscountPrice.HasValue && src.Price != 0)
                        {
                            return (src.Price - src.DiscountPrice.Value) / src.Price * 100;
                        }
                        else
                        {
                            return 0;
                        }
                    });
                })
                .ForMember(dest => dest.ReviewsQuantity, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductRates != null && src.ProductRates.Count != 0)
                        {
                            return src.ProductRates.Count;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.GeneralRate, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductRates != null && src.ProductRates.Count != 0)
                        {
                            return src.ProductRates.Sum(pr => pr.Mark) / src.ProductRates.Count;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.RatingStats, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductRates != null && src.ProductRates.Count != 0)
                        {
                            var totalRates = src.ProductRates.Count;
                            var rates = src.ProductRates
                                .GroupBy(pr => pr.Mark)
                                .Select(group => new RatingStat
                                {
                                    mark = group.Key,
                                    percent = (int)(group.Count() * 100.0 / totalRates)
                                })
                                .OrderByDescending(r => r.mark)
                                .ToList();
                            return rates;
                        }
                        return null;
                    });
                });
            CreateMap<ProductProperty, ProductPropProfile>();
            CreateMap<AboutProductItem, AboutProductProfile>();
        }
    }
}
