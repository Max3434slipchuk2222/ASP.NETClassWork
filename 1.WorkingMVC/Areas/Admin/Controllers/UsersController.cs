using Microsoft.AspNetCore.Mvc;
using _1.WorkingMVC.Interfaces;
using Microsoft.AspNetCore.Authorization;
using _1.WorkingMVC.Constants;
using _1.WorkingMVC.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using _1.WorkingMVC.Areas.Admin.Models.Users;


namespace _1.WorkingMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin}")]
public class UsersController(
	IUserService userService,
	UserManager<UserEntity> userManager,
	RoleManager<RoleEntity> roleManager,
	IMapper mapper) : Controller
{
	public async Task<IActionResult> Index()
	{
		var result = await userService.GetUsersAsync();
		return View(result);
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return NotFound();
		}
		var model = mapper.Map<UserEditViewModel>(user);
		model.AllRoles = await roleManager.Roles
			.Select(r => r.Name!)
			.ToListAsync();

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(UserEditViewModel model)
	{
		if (!ModelState.IsValid)
		{
			model.AllRoles = await roleManager.Roles
				.Select(r => r.Name!)
				.ToListAsync();
			return View(model);
		}

		var user = await userManager.FindByIdAsync(model.Id.ToString());
		if (user == null)
		{
			return NotFound();
		}
		user.FirstName = model.FirstName;
		user.LastName = model.LastName;
		user.Email = model.Email;
		user.UserName = model.Email;

		await userManager.UpdateAsync(user);

		var userRoles = await userManager.GetRolesAsync(user);
		var rolesAdd = model.SelectedRoles.Except(userRoles);
		await userManager.AddToRolesAsync(user, rolesAdd);
		var rolesRemove = userRoles.Except(model.SelectedRoles);
		await userManager.RemoveFromRolesAsync(user, rolesRemove);

		return RedirectToAction(nameof(Index));
	}
}