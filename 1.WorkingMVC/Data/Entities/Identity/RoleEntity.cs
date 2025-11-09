using Microsoft.AspNetCore.Identity;

namespace _1.WorkingMVC.Data.Entities.Identity;

// Клас описує роль користувача
public class RoleEntity : IdentityRole<int>
{
	public RoleEntity() { }

	public RoleEntity(string name) { this.Name = name; }

	public ICollection<UserRoleEntity> UserRoles { get; set; } = null!;
}

