using _1.WorkingMVC.Data;
using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Areas.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using _1.WorkingMVC.Repositories;

namespace _1.WorkingMVC.Areas.Admin.Repositories
{
	public class CategoryRepository : BaseRepository<CategoryEntity, int>, ICategoryRepository
	{
		public CategoryRepository(MyAppDbContext dbContext) : base(dbContext)
		{
		}
		public override async Task<CategoryEntity?> GetByIdAsync(int id)
		{
			return await _dbSet.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
		}
		public override async Task<IEnumerable<CategoryEntity>> GetAllAsync(bool isDeleted = false)
		{
			return await _dbSet.Where(x => x.IsDeleted == isDeleted).ToListAsync();
		}
		public Task<CategoryEntity> FindByNameAsync(string name, int? actualId = null)
		{
			var nameTrim = name.Trim().ToLower();
			var entity = _dbSet
				.Where(c => c.Name.ToLower() == nameTrim && !c.IsDeleted);
			if (actualId.HasValue)
			{
				entity = entity.Where(c => c.Id != actualId.Value);
			}
			return entity.FirstOrDefaultAsync();
		}
	}
}
