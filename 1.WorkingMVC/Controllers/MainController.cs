using _1.WorkingMVC.Data;
using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Models.Category;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1.WorkingMVC.Controllers;

//.NEt 8.0 та 9.0
public class MainController(MyAppDbContext myAppDbContext,
	IConfiguration configuration, IMapper mapper, IImageService imageService) : Controller
{
	public async Task<IActionResult> Index()
	{
		var list = await myAppDbContext.Categories
			.Where(c => !c.IsDeleted)
			.OrderBy(c => c.DateCreated)
			.ProjectTo<CategoryItemModel>(mapper.ConfigurationProvider)
			.ToListAsync();
		return View(list);
	}

	//Для того, щоб побачити сторінку створення категорії
	[HttpGet] //Щоб побачити сторінку і внести інформацію про категорію
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost] //Збереження даних
	public async Task<IActionResult> Create(CategoryCreateModel model)
	{
		if(!ModelState.IsValid)
		{
			return View(model);
		}

		var name = model.Name.Trim();
		var entity = myAppDbContext.Categories
			.SingleOrDefault(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted);
		if(entity != null)
		{
			ModelState.AddModelError("Name",
				$"Категорія з назвою '{name}' вже існує.");
			return View(model);
		}

	    entity = new CategoryEntity
		{
			Name = model.Name
		};
		var dirImageName = configuration.GetValue<string>("DirImageName");
		if (model.Image != null)
		{
			entity.Image = await imageService.UploadImageAsync(model.Image);
		}
		myAppDbContext.Categories.Add(entity);
		myAppDbContext.SaveChanges();
		return RedirectToAction(nameof(Index));
	}
	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var entity = await myAppDbContext.Categories
			.Where(c => c.Id == id && !c.IsDeleted) 
			.SingleOrDefaultAsync();

		if (entity == null)
		{
			return NotFound(); 
		}

		var model = mapper.Map<CategoryEditModel>(entity);
		return View(model);
	}
	[HttpPost]
	public async Task<IActionResult> Edit(CategoryEditModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var entity = await myAppDbContext.Categories.FindAsync(model.Id);

		if (entity == null || entity.IsDeleted)
		{
			return NotFound();
		}

		var name = model.Name.Trim();

		var repeat = await myAppDbContext.Categories
			.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != model.Id && !c.IsDeleted);

		if (repeat != null)
		{
			ModelState.AddModelError("Name", $"Категорія з назвою '{name}' вже існує.");
		
			return View(model);
		}

		entity.Name = name;

		if (model.NewImage != null)
		{
			entity.Image = await imageService.UploadImageAsync(model.NewImage);
		}

		myAppDbContext.Categories.Update(entity);
		await myAppDbContext.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		var entity = await myAppDbContext.Categories.FindAsync(id);
		if (entity == null)
		{
			return NotFound();
		}

		entity.IsDeleted = true;
		myAppDbContext.Categories.Update(entity);
		await myAppDbContext.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}

}