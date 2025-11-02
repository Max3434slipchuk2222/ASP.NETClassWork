using System.ComponentModel.DataAnnotations;

namespace _1.WorkingMVC.Models.Category
{
	public class CategoryCreateModel
	{
		[Display(Name = "Назва")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Фото")]
		public IFormFile? Image { get; set; }
	}
}
