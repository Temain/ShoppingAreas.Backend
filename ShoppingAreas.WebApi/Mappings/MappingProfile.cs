using AutoMapper;
using ShoppingAreas.Services.Models;
using ShoppingAreas.WebApi.ViewModels;

namespace ShoppingAreas.WebApi.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<AreaView, VmArea>();
		}
	}
}
