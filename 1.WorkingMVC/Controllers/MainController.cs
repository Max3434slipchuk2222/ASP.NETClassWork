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
	ICategoryService categoryService, IConfiguration configuration, IMapper mapper, IImageService imageService) : Controller
{
	public async Task<IActionResult> Index()
	{
		var model = await categoryService.GetAllAsync();
		return View(model);
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

		try
		{
			await categoryService.CreateAsync(model);
			return RedirectToAction(nameof(Index));
		}
		catch (InvalidOperationException ex) 
		{
			ModelState.AddModelError("Name", ex.Message);
			return View(model);
		}
	}
	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var model = await categoryService.GetEditAsync(id);
		if (model == null)
		{
			return NotFound();
		}
		return View(model);
	}
	[HttpPost]
	public async Task<IActionResult> Edit(CategoryEditModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}
		try
		{
			await categoryService.UpdateAsync(model);
			return RedirectToAction(nameof(Index));
		}
		catch (InvalidOperationException ex)
		{
			ModelState.AddModelError("Name", ex.Message);
			return View(model);
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}
	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			await categoryService.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

}