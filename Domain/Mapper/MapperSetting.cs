using AutoMapper;
using Servize.Domain.Model;
using Servize.DTO.PROVIDER;

namespace Servize.Domain.Mapper
{
    public class MapperSetting : Profile
    {
        public MapperSetting()
        {
            CreateMap<ServizeProvider, ServizeProviderDTO>().ReverseMap();
        }
    }
}
