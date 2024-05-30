using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Models;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Profiles.JwtTokenProfiles;
using static System.Net.WebRequestMethods;

namespace amazon_backend.Profiles
{
    public class MappingProfile : Profile
    {
        private readonly string? bucketUrl;
        public MappingProfile()
        {
            bucketUrl = "https://perry11.s3.eu-north-1.amazonaws.com/";
            #region JwtToken
            CreateMap<JwtToken, JwtTokenProfile>();
            #endregion

            #region User
            CreateMap<User, ReviewUserProfile>()
                .ForMember(dest => dest.AvatarUrl, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.AvatarUrl != null)
                        {
                            return Path.Combine(bucketUrl, src.AvatarUrl);
                        }
                        return null;
                    });
                })
                .ForMember(dest => dest.FirstName, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.FirstName == null)
                        {
                            if (src.DeletedAt != null) return "Deleted";
                            else return "New";
                        }
                        return src.FirstName;
                    });
                })
                .ForMember(dest => dest.LastName, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.LastName == null)
                        {
                            return "User";
                        }
                        return src.LastName;
                    });
                });


            CreateMap<User, ClientProfile>()
                .ForMember(dest => dest.AvatarUrl, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.AvatarUrl != null)
                        {
                            return Path.Combine(bucketUrl, src.AvatarUrl);
                        }
                        return null;
                    });
                })
                 .ForMember(dest => dest.IsDeleted, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         if (src.DeletedAt != null)
                         {
                             return true;
                         }
                         return false;
                     });
                 })
                  .ForMember(dest => dest.IsAdmin, opt =>
                  {
                      opt.MapFrom((src, dest, destMember, context) =>
                      {
                          if (src.Role == "Admin")
                          {
                              return true;
                          }
                          return false;
                      });
                  });
            #endregion

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
                        return Path.Combine(bucketUrl, src.ImageUrl);
                    });
                });
            CreateMap<ReviewTag, ReviewTagProfile>();
            CreateMap<Review, ReviewProfile>();
            #endregion
        }
    }
}
