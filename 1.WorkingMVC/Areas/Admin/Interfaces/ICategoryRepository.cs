using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Interfaces;

namespace _1.WorkingMVC.Areas.Admin.Interfaces;

public interface ICategoryRepository : IGenericRepository<CategoryEntity, int>
{
	 Task<CategoryEntity?> FindByNameAsync(string name, int? actualId = null);
}
