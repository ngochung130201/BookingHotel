using AutoMapper;
using BusinessLogic.Dtos.Function;
using BusinessLogic.Dtos.Role;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Dtos.Service;
using BusinessLogic.Dtos.User;
using BusinessLogic.Entities;
using BusinessLogic.Entities.Identity;

namespace BusinessLogic.Mappings
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            CreateMap<AppUser, UserDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleResponse>().ReverseMap();
            CreateMap<Function, FunctionResponse>().ReverseMap();
            CreateMap<Function, FunctionDetailDto>().ReverseMap();
            CreateMap<FunctionWithRoleDto, Permission>().ReverseMap();
            CreateMap<RoomTypes, RoomTypesDto>().ReverseMap();
            CreateMap<RoomTypes, RoomTypesResponse>().ReverseMap();
            CreateMap<Entities.Services, ServiceDto>().ReverseMap();
            CreateMap<Entities.Services, ServiceResponse>().ReverseMap();
            CreateMap<Rooms, RoomsDto>().ReverseMap();
            CreateMap<Rooms, RoomsResponse>().ReverseMap();
        }
    }
}
