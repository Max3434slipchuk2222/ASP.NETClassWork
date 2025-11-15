using System.ComponentModel.DataAnnotations;

namespace _1.WorkingMVC.Areas.Admin.Models.Category
{
	public class CategoryCreateModel
	{
		[Display(Name = "Назва")]
		[Required(ErrorMessage = "Вкажіть назву категорії")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Фото")]
		[Required(ErrorMessage = "Вкажіть фото для категорії")]
		public IFormFile? Image { get; set; }
	}
}
