using _1.WorkingMVC.Models.Category;

namespace _1.WorkingMVC.Interfaces;

public interface ICategoryService
{
	Task<List<CategoryItemModel>> GetAllAsync();
}
