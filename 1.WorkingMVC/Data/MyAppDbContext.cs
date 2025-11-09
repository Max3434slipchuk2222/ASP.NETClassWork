using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _1.WorkingMVC.Data;

public class MyAppDbContext : IdentityDbContext<UserEntity, RoleEntity,
	int, IdentityUserClaim<int>, UserRoleEntity, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
	public MyAppDbContext(DbContextOptions<MyAppDbContext> dbContextOptions)
		: base(dbContextOptions)
	{ }
	// DbSet - відповідає за таблиці. Це створення таблиці
	public DbSet<CategoryEntity> Categories { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<UserRoleEntity>()
			.HasOne(ur => ur.User)
			.WithMany(u => u.UserRoles)
			.HasForeignKey(ur => ur.UserId);
		builder.Entity<UserRoleEntity>()
			.HasOne(ur => ur.Role)
			.WithMany(u => u.UserRoles)
			.HasForeignKey(ur => ur.RoleId);

	}
}
