using System.ComponentModel.DataAnnotations;
namespace _1.WorkingMVC.Models.Account
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "Вкажіть пошту")]
		[EmailAddress(ErrorMessage = "Некоректна пошта")]
		[Display(Name = "Пошта")]
		public string Email { get; set; } = string.Empty;
	}
}