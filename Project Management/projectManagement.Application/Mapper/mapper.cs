using AutoMapper;
using projectManagement.API.Models;
using projectManagement.Application.DTO;

namespace projectManagement.Application.Mapper;

public class Mapper : Profile
{ 
    public Mapper()
    {
        CreateMap<Domain.Entities.User, DTO.UserDto>().ReverseMap();
        CreateMap<Domain.Entities.User, User>().ReverseMap();
        CreateMap<Domain.Entities.User, CreateUser>().ReverseMap();
        CreateMap<Domain.Entities.Project, ProjectDto>().ReverseMap();
        CreateMap<Domain.Entities.Project, CreateProject>().ReverseMap();
        CreateMap<Domain.Entities.Tasks, CreateTaskRequest>().ReverseMap();
        CreateMap<Domain.Entities.Tasks, UpdateTaskRequest>().ReverseMap();
        CreateMap<TasksDto, UpdateTaskRequest>().ReverseMap();
        CreateMap<TasksDto, Domain.Entities.Tasks>().ReverseMap();

    }
}
