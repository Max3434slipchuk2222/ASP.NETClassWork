using _1.WorkingMVC.Data;
using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Data.Entities.Identity;
using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Repositories;
using _1.WorkingMVC.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using _1.WorkingMVC.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyAppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Використовуємо Identity з нашими класами UserEntity та RoleEntity
builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
	//Встановлюємо правила для паролів
	options.Password.RequireDigit = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;
})
	//Налаштовуємо збереження користувачів та ролей у базі даних
	.AddEntityFrameworkStores<MyAppDbContext>()
	.AddDefaultTokenProviders(); // Додавання сервісу для генерації токенів
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ICategoryRepository ,CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<_1.WorkingMVC.Areas.Admin.Interfaces.ICategoryService, _1.WorkingMVC.Areas.Admin.Services.CategoryService>();
builder.Services.AddScoped<_1.WorkingMVC.Areas.Admin.Interfaces.IImageService, _1.WorkingMVC.Areas.Admin.Services.ImageService>();
builder.Services.AddScoped<_1.WorkingMVC.Areas.Admin.Interfaces.ICategoryRepository, _1.WorkingMVC.Areas.Admin.Repositories.CategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization(); // Встановлює чи має користувач доступ до сторінки

app.MapStaticAssets();
app.MapAreaControllerRoute(
	name: "MyAreaPigAdmin",
	areaName: "Admin",
	pattern: "admin/{controller=Dashboards}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}")
    .WithStaticAssets();

var dirImageName = builder.Configuration.GetValue<string>("DirImageName") ?? "test";

Console.WriteLine(dirImageName);
var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName);
Directory.CreateDirectory(dirImageName);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dirImageName}"
});
// Ініціалізація бази даних та додавання ролей при початковому запуску
using (var scoped = app.Services.CreateScope())
{
	var myAppDbContext = scoped.ServiceProvider.GetRequiredService<MyAppDbContext>();
	var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
	var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
	using var httpClient = new HttpClient();
	async Task<string> SaveImageAsync(string imageUrl)
	{
		try
		{
			var response = await httpClient.GetAsync(imageUrl);
			if (response.IsSuccessStatusCode)
			{
				var fileBytes = await response.Content.ReadAsByteArrayAsync();

				var extension = Path.GetExtension(imageUrl).Split('?')[0];
				if (string.IsNullOrEmpty(extension)) extension = ".jpg";

				var fileName = $"{Guid.NewGuid()}{extension}";
				var savePath = Path.Combine(path, fileName);

				await File.WriteAllBytesAsync(savePath, fileBytes);
				Console.WriteLine($"--> Завантажено фото: {fileName}");
				return fileName;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error downloading image: {ex.Message}");
		}
		return "default.jpg";
	}
	myAppDbContext.Database.Migrate();
	
	if (!myAppDbContext.Categories.Any())
	{
		var categories = new List<CategoryEntity>{
			new CategoryEntity {
				Name = "Напої безалкогольні",
				Image = await SaveImageAsync("https://src.zakaz.atbmarket.com/cache/category/%D0%91%D0%B5%D0%B7%D0%B0%D0%BB%D0%BA%D0%BE%D0%B3%D0%BE%D0%BB%D1%8C%D0%BD%D1%96%20%D0%BD%D0%B0%D0%BF%D0%BE%D1%96%CC%88.webp")
			},
			new CategoryEntity
			{
				Name = "Овочі та фрукти",
				Image = await SaveImageAsync("https://src.zakaz.atbmarket.com/cache/category/%D0%9E%D0%B2%D0%BE%D1%87%D1%96%20%D1%82%D0%B0%20%D1%84%D1%80%D1%83%D0%BA%D1%82%D0%B8.webp")
			}
		};
		myAppDbContext.Categories.AddRange(categories);
		myAppDbContext.SaveChanges();
	}

	if (!myAppDbContext.Roles.Any())
	{
		foreach (var roleName in Roles.AllRoles)
		{
			var role = new RoleEntity(roleName);
			var result = roleManager.CreateAsync(role).Result;
			if (result.Succeeded)
			{
				Console.WriteLine($"-----Створили роль {roleName}-----");
			}
			else
			{
				Console.WriteLine($"-----Помилка при створенні ролі {roleName}-----");

				foreach (var error in result.Errors)
				{
					Console.WriteLine($"Помилка: {error.Description}");
				}
			}



		}
	}
	if (!userManager.Users.Any())
	{
		var admin = new UserEntity { UserName = "admin123", Email = "admin123@gmail.com", EmailConfirmed = true };
		var user = new UserEntity { UserName = "user123", Email = "user123@gmail.com", EmailConfirmed = true };

		await userManager.CreateAsync(admin, "987654321");
		await userManager.AddToRoleAsync(admin, Roles.Admin);

		await userManager.CreateAsync(user, "987654321");
		await userManager.AddToRoleAsync(user, Roles.User);
	}
	if (!myAppDbContext.OrderStatuses.Any())
	{
		List<string> names = new List<string>() {
			   "Нове", "Очікує оплати", "Оплачено",
			   "В обробці", "Готується до відправки",
			   "Відправлено", "У дорозі", "Доставлено",
			   "Завершено", "Скасовано (вручну)", "Скасовано (автоматично)",
			   "Повернення", "В обробці повернення" };

		var orderStatuses = names.Select(name => new OrderStatusEntity { Name = name }).ToList();

		await myAppDbContext.OrderStatuses.AddRangeAsync(orderStatuses);
		await myAppDbContext.SaveChangesAsync();
	}
	if (!myAppDbContext.Products.Any())
	{
		var categoryName1 = myAppDbContext.Categories.First(c => c.Name == "Напої безалкогольні");
		var categoryName2 = myAppDbContext.Categories.First(c => c.Name == "Овочі та фрукти");

		var products = new List<ProductEntity>
		{
			new ProductEntity {
				Name = "Сік фруктовий",
				Description = "Смачний та корисний",
				Price = 89,
				CategoryId = categoryName1.Id
			},
			new ProductEntity {
				Name = "Pepsi",
				Description = "Освіжаючий напій",
				Price = 123,
				CategoryId = categoryName1.Id
			},
			new ProductEntity {
				Name = "Картопля(1 кг)",
				Description = "Універсальний сорт із середнім вмістом крохмалі",
				Price = 30,
				CategoryId = categoryName2.Id
			}
		};
		await myAppDbContext.Products.AddRangeAsync(products);
		await myAppDbContext.SaveChangesAsync();
	}
	if (!myAppDbContext.ProductImages.Any())
	{
		var juise = myAppDbContext.Products.First(p => p.Name == "Сік фруктовий");
		var pepsi = myAppDbContext.Products.First(p => p.Name == "Pepsi");
		var potatoes = myAppDbContext.Products.First(p => p.Name == "Картопля(1 кг)");

		var images = new List<ProductImageEntity>
		{
			new ProductImageEntity { Name = await SaveImageAsync("https://galicia.com.ua/uploads/123/sTvyCzCr2mvFdUEEddekmz6i1yyLXD-metaNjYzNDJERUQtMEY5OC00RDgwLUJGOEEtODQ5MDZGNzFFNjgxLndlYnA=-.webp"), Priority = 1, ProductId = juise.Id },
			new ProductImageEntity { Name = await SaveImageAsync("https://images.silpo.ua/products/300x300/webp/f9132917-8574-43ee-a34b-4cfed20b6cff.png"), Priority = 1, ProductId = pepsi.Id },
			new ProductImageEntity { Name = await SaveImageAsync("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ4Dx1d2IR3C-8ME1VljpnlCwCBMkhqbLWs5w&s"), Priority = 1, ProductId = potatoes.Id }
		};
		await myAppDbContext.ProductImages.AddRangeAsync(images);
		await myAppDbContext.SaveChangesAsync();
	}
	if (!myAppDbContext.Carts.Any())
	{
		var user = userManager.FindByEmailAsync("user123@gmail.com").Result;
		var product = myAppDbContext.Products.First();

		if (user != null)
		{
			var cartItem = new CartEntity
			{
				UserId = user.Id,
				ProductId = product.Id,
				Quantity = 2
			};
			await myAppDbContext.Carts.AddAsync(cartItem);
			await myAppDbContext.SaveChangesAsync();
		}
	}
	if (!myAppDbContext.Orders.Any())
	{
		var user = userManager.FindByEmailAsync("user123@gmail.com").Result;
		var status = myAppDbContext.OrderStatuses.First(s => s.Name == "Нове");
		var product1 = myAppDbContext.Products.OrderBy(p => p.Id).First();
		var product2 = myAppDbContext.Products.OrderBy(p => p.Id).Last();

		if (user != null)
		{
			var order = new OrderEntity
			{
				UserId = user.Id,
				OrderStatusId = status.Id,
				DateCreated = DateTime.UtcNow
			};

			await myAppDbContext.Orders.AddAsync(order);
			await myAppDbContext.SaveChangesAsync();

			var orderItems = new List<OrderItemEntity>
			{
				new OrderItemEntity
				{
					OrderId = order.Id,
					ProductId = product1.Id,
					PriceBuy = product1.Price,
					Count = 1
				},
				new OrderItemEntity
				{
					OrderId = order.Id,
					ProductId = product2.Id,
					PriceBuy = product2.Price,
					Count = 2
				}
			};

			await myAppDbContext.OrderItems.AddRangeAsync(orderItems);
			await myAppDbContext.SaveChangesAsync();
		}
	}


	
}
app.Run();