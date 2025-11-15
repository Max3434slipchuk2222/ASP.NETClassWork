using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Data.Entities.Identity;
using _1.WorkingMVC.Models.Account;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _1.WorkingMVC.Constants;


namespace _1.WorkingMVC.Controllers;

public class AccountController(
	UserManager<UserEntity> userManager,
	IImageService imageService,
	IMapper mapper,
	SignInManager<UserEntity> signInManager,
	IEmailService emailService) : Controller
{
	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}
		var user = await userManager.FindByEmailAsync(model.Email);
		if (user != null)
		{
			var res = await signInManager
				.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

			if (res.Succeeded)
			{
				return RedirectToAction("Index", "Main");
			}
		}
		ModelState.AddModelError("", "Дані вказано не вірно!");
		return View(model);
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}
		var user = mapper.Map<UserEntity>(model);

		if (model.Image != null)
		{
			user.Image = await imageService.UploadImageAsync(model.Image);
		}

		var result = await userManager.CreateAsync(user, model.Password);

		if (result.Succeeded)
		{
			result = await userManager.AddToRoleAsync(user, Roles.User);
			await signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction("Index", "Main");
		}
		else
		{
			foreach (var item in result.Errors)
			{
				ModelState.AddModelError(string.Empty, item.Description);
			}
			return View(model);
		}
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();
		return RedirectToAction("Index", "Main");
	}

	[HttpGet]
	public IActionResult ForgotPassword()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await userManager.FindByEmailAsync(model.Email);
		if (user != null)
		{
			var token = await userManager.GeneratePasswordResetTokenAsync(user);
			var callbackUrl = Url.Action(
				"ResetPassword",
				"Account",
				new { email = user.Email, token = token },
				Request.Scheme);

			await emailService.SendEmailAsync(
				model.Email,
				"Скидання пароля",
				$"Перейдіть за цим посиланням для скидання вашого паролю: <a href='{callbackUrl}'>скинути пароль</a>");
			return RedirectToAction("ForgotPasswordConfirmation");
		}
		else
		{
			ModelState.AddModelError(string.Empty, "Користувача з таким email не знайдено в БД.");
			return View(model); 
		}
	}

	[HttpGet]
	public IActionResult ForgotPasswordConfirmation()
	{
		return View();
	}

	[HttpGet]
	public IActionResult ResetPassword(string email, string token)
	{
		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
		{
			ModelState.AddModelError("", "Некоректне посилання.");
			return View("Error");
		}

		var model = new ResetPasswordViewModel
		{
			Email = email,
			Token = token
		};
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await userManager.FindByEmailAsync(model.Email);
		if (user == null)
		{
			return RedirectToAction("ResetPasswordConfirmation");
		}

		var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

		if (result.Succeeded)
		{
			return RedirectToAction("ResetPasswordConfirmation");
		}
		else
		{
			return View(model);
		}
	}

	[HttpGet]
	public IActionResult ResetPasswordConfirmation()
	{
		return View();
	}
}