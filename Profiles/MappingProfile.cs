using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Data.Entity;
using AutoMapper;
using amazon_backend.Profiles.CategoryProfiles;

namespace amazon_backend.Profiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductCardProfile>();
            CreateMap<ProductImage,ProductImageProfile>();
            CreateMap<Category, CategoryViewProfile>();
            CreateMap<Product, ProductViewProfile>();
            CreateMap<ProductProperty, ProductPropProfile>();
            CreateMap<AboutProductItem, AboutProductProfile>();
        }
    }
}
