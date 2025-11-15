using System.ComponentModel.DataAnnotations;

namespace _1.WorkingMVC.Areas.Admin.Models.Category
{
	public class CategoryEditModel
	{
		public int Id { get; set; }

		[Display(Name = "Назва")]
		[Required(ErrorMessage = "Вкажіть нову назву категорії")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Введіть нове фото (залиште пустим, щоб не змінювати)")]
		public IFormFile? NewImage { get; set; }

		public string? CurrentImage { get; set; }
	}
}