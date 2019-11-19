using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDTO, User>();
            CreateMap<ImportProductDTO, Product>();
            CreateMap<ImportCategoryDTO, Category>();
            CreateMap<ImportCategoryProductDTO, CategoryProduct>();
            CreateMap<Product, ExportProductsInRangeDTO>();
            

        }
    }
}
