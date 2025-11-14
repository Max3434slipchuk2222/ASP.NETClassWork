using _1.WorkingMVC.Data.Entities.Identity;
using AutoMapper;
using _1.WorkingMVC.Models.Users;

namespace _1.WorkingMVC.Mappers;

public class UserMapper : Profile
{
	public UserMapper()
	{
		CreateMap<UserEntity, UserItemModel>()
			.ForMember(x => x.FullName, opt =>
				opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
			.ForMember(x => x.Image, opt =>
				opt.MapFrom(x => x.Image ?? "default.webp"))
			.ForMember(x => x.Roles, opt =>
				opt.MapFrom(x => x.UserRoles.Select(x => x.Role.Name).ToArray()));
		CreateMap<UserEntity, UserEditViewModel>()
			.ForMember(dest => dest.SelectedRoles, opt =>
				opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()))
			.ForMember(dest => dest.AllRoles, opt => opt.Ignore());
	}
}