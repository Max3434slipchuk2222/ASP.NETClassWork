using System.ComponentModel.DataAnnotations;

namespace _1.WorkingMVC.Models.Users;

public class UserEditViewModel
{
	public int Id { get; set; }

	[Display(Name = "Прізвище")]
	public string? LastName { get; set; }

	[Display(Name = "Ім'я")]
	public string? FirstName { get; set; }

	[Display(Name = "Електронна адреса")]
	public string Email { get; set; } = string.Empty;
	public List<string> AllRoles { get; set; } = new();
	public List<string> SelectedRoles { get; set; } = new();
}