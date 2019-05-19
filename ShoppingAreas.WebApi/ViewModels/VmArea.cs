using System;

namespace ShoppingAreas.WebApi.ViewModels
{
	public class VmArea
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public decimal TotalArea { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
