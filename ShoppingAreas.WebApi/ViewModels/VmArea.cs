using System;
using ShoppingAreas.Web.ViewModels;

namespace ShoppingAreas.WebApi.ViewModels
{
	public class VmArea
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public decimal TotalArea { get; set; }
		public string ImagePath { get; set; }
		public string ImageType { get; set; }
		public VmFile Image { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
