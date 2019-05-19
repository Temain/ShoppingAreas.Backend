using System;

namespace ShoppingAreas.Services.Models
{
	public class AreaView
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
