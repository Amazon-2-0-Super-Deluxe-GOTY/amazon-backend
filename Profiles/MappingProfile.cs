using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;
using amazon_backend.Profiles.ReviewProfiles;
using amazon_backend.Profiles.UserProfiles;
using amazon_backend.Profiles.JwtTokenProfiles;
using amazon_backend.Profiles.AboutProductProfiles;
using amazon_backend.Profiles.ProductImageProfiles;
using amazon_backend.Profiles.ProductPropertyProfiles;
using amazon_backend.Profiles.ReviewTagProfiles;
using amazon_backend.Profiles.ReviewImageProfiles;
using amazon_backend.Profiles.CartItemProfiles;
using amazon_backend.Profiles.CartProfiles;

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
                .ForMember(dest => dest.ProductImages, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductImages != null && src.ProductImages.Count != 0)
                        {
                            return src.ProductImages.OrderBy(pi => pi.CreatedAt);
                        }
                        return null;
                    });
                })
                .ForMember(dest => dest.GeneralRate, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.Reviews != null && src.Reviews.Count != 0)
                        {
                            return Math.Round(src.Reviews.Average(r => r.Mark), 1);
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
                         if (src.DiscountPercent.HasValue && src.DiscountPercent.Value > 0)
                         {
                             return src.Price * (1 - src.DiscountPercent.Value / 100.0);
                         }
                         else
                         {
                             return 0;
                         }
                     });
                 });
            CreateMap<Product, ProductViewProfile>()
                .ForMember(dest => dest.ProductImages, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductImages != null && src.ProductImages.Count != 0)
                        {
                            return src.ProductImages.OrderBy(pi => pi.CreatedAt);
                        }
                        return null;
                    });
                })
                .ForMember(dest => dest.DiscountPrice, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.DiscountPercent.HasValue && src.DiscountPercent.Value > 0)
                        {
                            return src.Price * (1 - src.DiscountPercent.Value / 100.0);
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
                            return Math.Round(src.Reviews.Average(r => r.Mark), 1);
                        }
                        return 0;
                    });
                });
            CreateMap<Product, ProductCartItemProfile>()
                .ForMember(dest => dest.ImageUrl, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ProductImages != null && src.ProductImages.Count != 0)
                        {
                            return Path.Combine(bucketUrl, src.ProductImages.First().ImageUrl);
                        }
                        return null;
                    });
                })
                .ForMember(dest => dest.DiscountPrice, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.DiscountPercent.HasValue && src.DiscountPercent.Value > 0)
                        {
                            return src.Price * (1 - src.DiscountPercent.Value / 100.0);
                        }
                        else
                        {
                            return 0;
                        }
                    });
                });
            #endregion

            #region ProductImage
            CreateMap<ProductImage, ProductImageCardProfile>()
           .ForMember(dest => dest.ImageUrl, opt =>
           {
               opt.MapFrom((src, dest, destMember, context) =>
               {
                   return Path.Combine(bucketUrl, src.ImageUrl);
               });
           });
            CreateMap<ProductImage, ProductImageProfile>()
            .ForMember(dest => dest.ImageUrl, opt =>
            {
                opt.MapFrom((src, dest, destMember, context) =>
                {
                    return Path.Combine(bucketUrl, src.ImageUrl);
                });
            });
            #endregion

            #region ProductProperty
            CreateMap<ProductProperty, ProductPropProfile>();
            CreateMap<ProductProperty, ProductPropFormProfile>()
                .ForMember(dest => dest.name, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return src.Key;
                    });
                })
                .ForMember(dest => dest.text, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return src.Value;
                    });
                });
            #endregion

            #region AboutProductItem
            CreateMap<AboutProductItem, AboutProductProfile>();
            CreateMap<AboutProductItem, AboutProductFormProfile>()
                .ForMember(dest => dest.name, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return src.Title;
                    });
                })
                .ForMember(dest => dest.text, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return src.Text;
                    });
                });
            #endregion

            #region Category
            CreateMap<Category, CategoryViewProfile>();
            CreateMap<Category, CategoryProductProfile>();
            #endregion

            #region Review
            CreateMap<Review, ReviewProfile>()
                .ForMember(dest => dest.Likes, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (src.ReviewLikes != null && src.ReviewLikes.Count != 0)
                        {
                            return src.ReviewLikes.Count;
                        }
                        return 0;
                    });
                });
            #endregion

            #region ReviewTag
            CreateMap<ReviewTag, ReviewTagProfile>();
            #endregion

            #region ReviewImage
            CreateMap<ReviewImage, ReviewImageProfile>()
                .ForMember(dest => dest.ImageUrl, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return Path.Combine(bucketUrl, src.ImageUrl);
                    });
                });
            #endregion

            #region CategoryImage
            CreateMap<CategoryImage, CategoryImageProfile>()
                 .ForMember(dest => dest.ImageUrl, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         return Path.Combine(bucketUrl, src.ImageUrl);
                     });
                 });
            #endregion

            #region Cart
            CreateMap<Cart, CartProfile>()
                 .ForMember(dest => dest.TotalPrice, opt =>
                 {
                     opt.MapFrom((src, dest, destMember, context) =>
                     {
                         if (dest.CartItems != null && dest.CartItems.Count != 0)
                         {
                             return dest.CartItems.Sum(ci => ci.Price);
                         }
                         return 0;
                     });
                 });
            #endregion

            #region CartItem
            CreateMap<CartItem, CartItemProfile>()
                .ForMember(dest => dest.Price, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        return (src.Product!.Price * (1 - (src.Product.DiscountPercent.HasValue ? src.Product.DiscountPercent.Value : 0) / 100.0)) * dest.Quantity;
                    });
                });
            #endregion

        }
    }
}
