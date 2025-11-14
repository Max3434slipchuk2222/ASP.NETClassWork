using _1.WorkingMVC.Models.Users;

namespace _1.WorkingMVC.Interfaces;

public interface IUserService
{
	Task<List<UserItemModel>> GetUsersAsync();
}