using _1.WorkingMVC.Areas.Admin.Models.Category;

namespace _1.WorkingMVC.Areas.Admin.Interfaces;

public interface ICategoryService
{
	Task<List<CategoryItemModel>> GetAllAsync();
	Task<CategoryEditModel?> GetEditAsync(int id);
	Task CreateAsync(CategoryCreateModel model);
	Task UpdateAsync(CategoryEditModel model);
	Task DeleteAsync(int id);
}
