using _1.WorkingMVC.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Models.Users;

namespace _1.WorkingMVC.Services;

public class UserService(MyAppDbContext context,
	IMapper mapper) : IUserService
{
	public async Task<List<UserItemModel>> GetUsersAsync()
	{
		//Це sql запит
		var query = context.Users;
		var model = await query
			.ProjectTo<UserItemModel>(mapper.ConfigurationProvider)
			.ToListAsync();

		return model;
	}
}