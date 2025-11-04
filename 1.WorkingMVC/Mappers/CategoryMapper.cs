using _1.WorkingMVC.Data.Entities;
using _1.WorkingMVC.Models.Category;
using AutoMapper;

namespace _1.WorkingMVC.Mappers;

public class CategoryMapper : Profile
{
	public CategoryMapper()
	{
		CreateMap<CategoryEntity, CategoryItemModel>();
		CreateMap<CategoryEntity, CategoryEditModel>()
			.ForMember(dest => dest.CurrentImage, opt => opt.MapFrom(src => src.Image));
	}
}