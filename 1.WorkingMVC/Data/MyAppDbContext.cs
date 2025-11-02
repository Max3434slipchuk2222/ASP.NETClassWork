using _1.WorkingMVC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace _1.WorkingMVC.Data;

public class MyAppDbContext : DbContext
{
	public MyAppDbContext(DbContextOptions<MyAppDbContext> dbContextOptions)
		: base(dbContextOptions)
	{ }
	// DbSet - відповідає за таблиці. Це створення таблиці
	public DbSet<CategoryEntity> Categories { get; set; }
}
