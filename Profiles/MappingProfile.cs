using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;

namespace amazon_backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductCardProfile>();
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
                .ForMember(dest=>dest.GeneralRate,opt=>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if(src.ProductRates!=null&&src.ProductRates.Count!=0)
                        {
                            return src.ProductRates.Sum(pr => pr.Mark) / src.ProductRates.Count;
                        }
                        return 0;
                    });
                });
            CreateMap<ProductProperty, ProductPropProfile>();
            CreateMap<AboutProductItem, AboutProductProfile>();
        }
    }
}
