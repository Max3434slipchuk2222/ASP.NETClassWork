using _1.WorkingMVC.Data.Entities;

namespace _1.WorkingMVC.Interfaces;

public interface ICategoryRepository : IGenericRepository<CategoryEntity, int>
{
	 Task<CategoryEntity?> FindByNameAsync(string name, int? actualId = null);
}
