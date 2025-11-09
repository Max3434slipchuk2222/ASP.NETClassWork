using _1.WorkingMVC.Data.Entities.Identity;
using _1.WorkingMVC.Models.Account;
using AutoMapper;

namespace WorkingMVC.Mappers;

public class AccountMapper : Profile
{
	public AccountMapper()
	{
		CreateMap<RegisterViewModel, UserEntity>()
			.ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email))
			.ForMember(x => x.Image, opt => opt.Ignore());
	}
}
