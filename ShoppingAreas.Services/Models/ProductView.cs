using System;

namespace ShoppingAreas.Services.Models
{
	public class ProductView
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
