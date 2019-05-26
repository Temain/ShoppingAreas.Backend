using System;

namespace ShoppingAreas.Services.Models
{
	public class AreaView
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public decimal TotalArea { get; set; }
		public string ImagePath { get; set; }
		public string ImageType { get; set; }
		public string ImageHash { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
