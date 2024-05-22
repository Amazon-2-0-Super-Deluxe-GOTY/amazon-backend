using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;

namespace amazon_backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Product
            CreateMap<Product, ProductCardProfile>()
                .ForMember(dest => dest.GeneralRate, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.Reviews != null && src.Reviews.Count != 0)
                        {
                            return src.Reviews.Sum(pr => pr.Mark) / src.Reviews.Count;
                        }
                        return 0;
                    });
                })
                 .ForMember(dest => dest.ReviewsCount, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         if (src.Reviews != null && src.Reviews.Count != 0)
                         {
                             return src.Reviews.Count;
                         }
                         return 0;
                     });
                 })
                 .ForMember(dest => dest.DiscountPrice, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         if (src.DiscountPercent.HasValue && src.Price > 0)
                         {
                             return src.Price * (1 - src.DiscountPercent.Value / 100.0);
                         }
                         else
                         {
                             return src.Price;
                         }
                     });
                 });
            CreateMap<ProductImage, ProductImageProfile>();
            CreateMap<Product, ProductViewProfile>()
                .ForMember(dest => dest.DiscountPrice, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.DiscountPercent.HasValue && src.Price > 0)
                        {
                            return src.Price * (1 - src.DiscountPercent.Value / 100.0);
                        }
                        else
                        {
                            return src.Price;
                        }
                    });
                })
                .ForMember(dest => dest.ReviewsQuantity, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.Reviews != null && src.Reviews.Count != 0)
                        {
                            return src.Reviews.Count;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.GeneralRate, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.Reviews != null && src.Reviews.Count != 0)
                        {
                            return src.Reviews.Sum(pr => pr.Mark) / src.Reviews.Count;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.RatingStats, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.Reviews != null && src.Reviews.Count != 0)
                        {
                            var totalRates = src.Reviews.Count;
                            var rates = src.Reviews
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
            #endregion

            #region Category
            CreateMap<Category, CategoryViewProfile>();
            #endregion

            #region Review
            CreateMap<ReviewImage, ReviewImageProfile>()
                .ForMember(dest => dest.ImageUrl, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return Path.Combine("https://perry11.s3.eu-north-1.amazonaws.com/", src.ImageUrl);
                    });
                });
            CreateMap<ReviewTag, ReviewTagProfile>();
            CreateMap<Review, ReviewProfile>();
            #endregion
        }
    }
}
