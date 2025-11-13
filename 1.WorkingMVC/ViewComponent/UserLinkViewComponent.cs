using _1.WorkingMVC.Data.Entities.Identity;
using _1.WorkingMVC.Models.Account;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1.WorkingMVC.ViewComponents;

public class UserLinkViewComponent(UserManager<UserEntity> userManager,
	IMapper mapper) : ViewComponent
{
	public IViewComponentResult Invoke()
	{
		var userName = User.Identity?.Name;
		var model = new UserLinkViewModel();

		if (userName != null)
		{
			var user = userManager.FindByEmailAsync(userName).Result;
			model = mapper.Map<UserLinkViewModel>(user);
		}

		return View(model);
	}
}