using AutoMapper;
using BusinessLogic.Dtos.Function;
using BusinessLogic.Dtos.Pages;
using BusinessLogic.Dtos.Partners;
using BusinessLogic.Dtos.Role;
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
            CreateMap<Pages, PagesDetailDto>().ReverseMap();
            CreateMap<Pages, PagesResponse>().ReverseMap();
            CreateMap<FunctionWithRoleDto, Permission>().ReverseMap();
            CreateMap<Partners, PartnerDetailDto>().ReverseMap();
            CreateMap<Partners, PartnerResponse>().ReverseMap();
        }
    }
}
