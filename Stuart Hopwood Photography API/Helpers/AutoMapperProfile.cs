using AutoMapper;
using Stuart_Hopwood_Photography_API.Dtos;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}