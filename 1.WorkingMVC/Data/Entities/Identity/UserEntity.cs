using Microsoft.AspNetCore.Identity;

namespace _1.WorkingMVC.Data.Entities.Identity;

// Клас описує користувача
public class UserEntity : IdentityUser<int>
{
	public string? FirstName { get; set; } = null;
	public string? LastName { get; set; } = null;
	public string? Image { get; set; } = null;
	public ICollection<UserRoleEntity> UserRoles { get; set; } = null!;
	public ICollection<CartEntity> Carts { get; set; } = null!;
	public ICollection<OrderEntity> Orders { get; set; } = null!;
}
