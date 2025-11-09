using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Interfaces;
using _1.WorkingMVC.Models.Category;
using _1.WorkingMVC.Repositories;
using AutoMapper;

namespace _1.WorkingMVC.Services
{
	public class CategoryService(ICategoryRepository categoryRepository, IImageService imageService, IMapper mapper) : ICategoryService
	{
		public async Task<List<CategoryItemModel>> GetAllAsync()
		{
			var listTest = await categoryRepository.GetAllAsync();
			var model = mapper.Map<List<CategoryItemModel>>(listTest);
			return model;
		}
		public async Task<CategoryEditModel?> GetEditAsync(int id)
		{
			var entity = await categoryRepository.GetByIdAsync(id);
			if (entity == null)
			{
				return null;
			}
			return mapper.Map<CategoryEditModel>(entity);
		}
		public async Task CreateAsync(CategoryCreateModel model)
		{
			var name = model.Name.Trim();
			var uniqueName = await categoryRepository.FindByNameAsync(name);
			if (uniqueName != null)
			{
				throw new InvalidOperationException($"Категорія з  такою назвою вже існує.");
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

			var uniqueName = await categoryRepository.FindByNameAsync(name, model.Id);
			if (uniqueName != null)
			{
				throw new InvalidOperationException($"Категорія з такою назвою  вже існує.");
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
