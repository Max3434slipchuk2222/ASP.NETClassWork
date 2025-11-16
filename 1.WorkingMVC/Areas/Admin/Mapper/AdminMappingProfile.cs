using AutoMapper;
using _1.WorkingMVC.Data.Entities; 
using _1.WorkingMVC.Areas.Admin.Models.Category; 

namespace _1.WorkingMVC.Areas.Admin.Mapper
{
	public class AdminMappingProfile : Profile
	{
		public AdminMappingProfile()
		{

			CreateMap<CategoryEntity, CategoryItemModel>();
			CreateMap<CategoryCreateModel, CategoryEntity>();

			CreateMap<CategoryEntity, CategoryEditModel>();
			CreateMap<CategoryEditModel, CategoryEntity>();
		}
	}
}