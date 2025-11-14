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
    var MyAppDbContext = scoped.ServiceProvider.GetRequiredService<MyAppDbContext>();
    var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
    MyAppDbContext.Database.Migrate();
	var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

	var allUsers = userManager.Users.ToList();
	Console.WriteLine($"Всього користувачів: {allUsers.Count}");
    foreach (var user in allUsers)
    {
        Console.WriteLine($" Email: {user.Email}, UserName: {user.UserName}, EmailConfirmed: {user.EmailConfirmed}");
    }
	if (!MyAppDbContext.Categories.Any())
    {
        var categories = new List<CategoryEntity>{
            new CategoryEntity {
                Name = "Напої безалкогольні",
                Image = "https://src.zakaz.atbmarket.com/cache/category/%D0%91%D0%B5%D0%B7%D0%B0%D0%BB%D0%BA%D0%BE%D0%B3%D0%BE%D0%BB%D1%8C%D0%BD%D1%96%20%D0%BD%D0%B0%D0%BF%D0%BE%D1%96%CC%88.webp",
            },
            new CategoryEntity
            {
                Name = "Овочі та фрукти",
                Image = "https://src.zakaz.atbmarket.com/cache/category/%D0%9E%D0%B2%D0%BE%D1%87%D1%96%20%D1%82%D0%B0%20%D1%84%D1%80%D1%83%D0%BA%D1%82%D0%B8.webp",
            }
        };
        MyAppDbContext.Categories.AddRange(categories);
        MyAppDbContext.SaveChanges();
    }

    if (!MyAppDbContext.Roles.Any())
    {
        string[] roles = { "Admin", "User" };
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
}


app.Run();
