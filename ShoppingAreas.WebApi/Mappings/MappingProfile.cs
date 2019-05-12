using AutoMapper;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services.Models;
using ShoppingAreas.WebApi.ViewModels;
using ShoppingAreas.WebApi.ViewModels.Account;

namespace ShoppingAreas.WebApi.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<VmRegistration, User>();

			CreateMap<AreaView, VmArea>();
			CreateMap<VmArea, AreaView>();

			CreateMap<EquipmentView, VmEquipment>();
			CreateMap<VmEquipment, EquipmentView>();

			CreateMap<ProductView, VmProduct>();
			CreateMap<VmProduct, ProductView>();
		}
	}
}
