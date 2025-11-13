using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Models.Category;
using AutoMapper;
namespace _1.WorkingMVC.Services
{
	public class CategoryService(ICategoryRepository categoryRepository, IImageService imageService, IMapper mapper) : ICategoryService
	{
		public async Task<List<CategoryItemModel>> GetAllAsync()
		{
			var listTest = await categoryRepository.GetAllAsync();
			return mapper.Map<List<CategoryItemModel>>(listTest);
		}

		public async Task<CategoryEditModel?> GetForEditAsync(int id)
		{
			var entity = await categoryRepository.GetByIdAsync(id);
			return entity == null ? null : mapper.Map<CategoryEditModel>(entity);
		}

		// Fix for CS0535: Implementing the missing method 'GetEditAsync' from ICategoryService
		public async Task<CategoryEditModel?> GetEditAsync(int id)
		{
			return await GetForEditAsync(id);
		}

		public async Task CreateAsync(CategoryCreateModel model)
		{
			var name = model.Name.Trim();
			var existing = await categoryRepository.FindByNameAsync(name);
			if (existing != null)
			{
				throw new InvalidOperationException($"Категорія з назвою '{name}' вже існує.");
			}

			var entity = mapper.Map<CategoryEntity>(model);
			if (model.Image != null)
			{
				entity.Image = await imageService.UploadImageAsync(model.Image);
			}

			await categoryRepository.AddAsync(entity);
			await categoryRepository.SaveChangesAsync();
		}

		public async Task UpdateAsync(CategoryEditModel model)
		{
			var name = model.Name.Trim();
			var repeat = await categoryRepository.FindByNameAsync(name, model.Id);
			if (repeat != null)
			{
				throw new InvalidOperationException($"Категорія з назвою '{name}' вже існує.");
			}

			var entity = await categoryRepository.GetByIdAsync(model.Id);
			if (entity == null)
			{
				throw new KeyNotFoundException("Категорію не знайдено.");
			}

			entity.Name = name;
			if (model.NewImage != null)
			{
				entity.Image = await imageService.UploadImageAsync(model.NewImage);
			}

			await categoryRepository.UpdateAsync(entity);
			await categoryRepository.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await categoryRepository.GetByIdAsync(id);
			if (entity == null)
			{
				throw new KeyNotFoundException("Категорію не знайдено.");
			}

			entity.IsDeleted = true;
			await categoryRepository.UpdateAsync(entity);
			await categoryRepository.SaveChangesAsync();
		}
	}
}