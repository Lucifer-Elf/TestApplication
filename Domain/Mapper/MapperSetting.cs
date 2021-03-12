using AutoMapper;
using Servize.Domain.Model;
using Servize.Domain.Model.Provider;
using Servize.DTO.PROVIDER;
using Servize.DTO.USER;

namespace Servize.Domain.Mapper
{
    public class MapperSetting : Profile
    {
        public MapperSetting()
        {
            CreateMap<Provider, ProviderDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Client, ClientDTO>().ReverseMap();
        }
    }
}
