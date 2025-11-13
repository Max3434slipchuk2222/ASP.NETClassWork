using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace _1.WorkingMVC.Models.Account
{
	public class ResetPasswordViewModel
	{
		[HiddenInput]
		public string Email { get; set; } = string.Empty;

		[HiddenInput]
		public string Token { get; set; } = string.Empty;

		[Required(ErrorMessage = "Це поле обов'язкове!")]
		[DataType(DataType.Password)]
		[Display(Name = "Новий пароль")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Це поле обов'язкове!")]
		[Compare("Password", ErrorMessage = "Паролі не співпадають")]
		[DataType(DataType.Password)]
		[Display(Name = "Підтвердити новий пароль")]
		public string PasswordConfirm { get; set; } = string.Empty;
	}
}