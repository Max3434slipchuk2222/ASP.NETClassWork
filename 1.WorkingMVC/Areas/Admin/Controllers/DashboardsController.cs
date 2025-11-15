using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _1.WorkingMVC.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using _1.WorkingMVC.Constants;

namespace _1.WorkingMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin}")]
public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
