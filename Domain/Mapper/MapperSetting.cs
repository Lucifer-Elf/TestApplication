using AutoMapper;
using Servize.Domain.Model.Provider;
using Servize.DTO.PROVIDER;

namespace Servize.Domain.Mapper
{
    public class MapperSetting : Profile
    {
        public MapperSetting()
        {
            CreateMap<ServizeProvider, ServizeProviderDTO>().ReverseMap();
            CreateMap<ServizeCategory, ServizeCategoryDTO>().ReverseMap();
            CreateMap<ServizeSubCategory, ServizeSubCategoryDTO>().ReverseMap();
        }
    }
}
