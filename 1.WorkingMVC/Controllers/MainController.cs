using _1.WorkingMVC.Data;
using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Models.Category;
using Microsoft.AspNetCore.Mvc;

namespace _1.WorkingMVC.Controllers;

public class MainController(MyAppDbContext myAppDbContext, IConfiguration configuration) : Controller
{    
	public IActionResult Index()  
	{  
		var list = myAppDbContext.Categories.ToList();            
		return View(list);      
	}

	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Create(CategoryCreateModel model)
	{
		var entity = new CategoryEntity
		{
			Name = model.Name,
		};
		var dirImageName = configuration.GetValue<string>("DirImageName");
		if(model.Image != null)
		{
			var fileName = Guid.NewGuid().ToString()+".jpg";
			var pathSave = Path.Combine(Directory.GetCurrentDirectory(), dirImageName ?? "images", fileName);
			using var stream = new FileStream(pathSave, FileMode.Create);
			model.Image.CopyTo(stream);
			entity.Image = fileName;
		}
		myAppDbContext.Categories.Add(entity);
		myAppDbContext.SaveChanges();
		return RedirectToAction(nameof(Index));
	}
}    
