using Microsoft.AspNetCore.Identity;

namespace _1.WorkingMVC.Data.Entities.Identity;

// Клас описує зв'язок між користувачем та роллю
public class UserRoleEntity : IdentityUserRole<int>
{
	public UserEntity User { get; set; } = null!;
	public RoleEntity Role { get; set; } = null!;


}
