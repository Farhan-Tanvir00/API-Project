using AutoMapper;

namespace WebAPI.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Shirt, ShirtDTO>().ReverseMap();
        }
    }
}
