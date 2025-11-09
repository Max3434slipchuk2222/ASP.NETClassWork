namespace _1.WorkingMVC.Interfaces;

public interface IGenericRepository<TEntity, TKey> where TEntity : IEntity<TKey>
{
	Task<TEntity> GetByIdAsync(int id);
	Task<IEnumerable<TEntity>> GetAllAsync(bool isDeleted = false);
	Task<IQueryable<TEntity>> GetAllQurableAsync();
	Task AddAsync(TEntity entity);
	Task UpdateAsync(TEntity entity);
	Task DeleteAsync(TEntity entity);

	Task<int> SaveChangesAsync();
}
