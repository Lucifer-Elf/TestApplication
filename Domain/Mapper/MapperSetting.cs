using AutoMapper;
using Servize.Domain.Model.Client;
using Servize.Domain.Model.Provider;
using Servize.DTO.PROVIDER;
using Servize.DTO.USER;

namespace Servize.Domain.Mapper
{
    public class MapperSetting : Profile
    {
        public MapperSetting()
        {
            CreateMap<ServizeProvider, ServizeProviderDTO>().ReverseMap();
            CreateMap<ServizeCategory, ServizeCategoryDTO>().ReverseMap();
            CreateMap<ServizeProduct, ServizeSubCategoryDTO>().ReverseMap();
            CreateMap<UserClient, UserClientDTO>().ReverseMap();
        }
    }
}
