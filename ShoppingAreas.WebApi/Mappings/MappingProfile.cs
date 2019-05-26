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

			CreateMap<EquipmentAreaView, VmEquipmentArea>();
			CreateMap<EquipmentView, VmEquipmentArea>()
				.ForMember(desc => desc.EquipmentId, opt => opt.MapFrom(src => src.Id))
				.ForMember(desc => desc.EquipmentName, opt => opt.MapFrom(src => src.Name))
				.ForMember(desc => desc.Count, opt => opt.MapFrom(src => 1));
			CreateMap<VmEquipmentArea, EquipmentAreaView>();

			CreateMap<ProductAreaView, VmProductArea>();
			CreateMap<ProductView, VmProductArea>()
				.ForMember(desc => desc.ProductId, opt => opt.MapFrom(src => src.Id))
				.ForMember(desc => desc.ProductName, opt => opt.MapFrom(src => src.Name));
			CreateMap<VmProductArea, ProductAreaView>();

			CreateMap<EquipmentView, VmEquipment>();
			CreateMap<VmEquipment, EquipmentView>();

			CreateMap<ProductView, VmProduct>();
			CreateMap<VmProduct, ProductView>();

			CreateMap<AreaReportView, VmAreaReport>();
		}
	}
}
