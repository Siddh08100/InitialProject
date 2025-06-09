using AutoMapper;
using projectManagement.API.Models;

namespace projectManagement.Application.Mapper;

public class mapper : Profile
{ 
    public mapper()
    {
        CreateMap<Domain.Entities.User, DTO.UserDto>().ReverseMap();
        CreateMap<Domain.Entities.User, User>().ReverseMap();
    }
}
