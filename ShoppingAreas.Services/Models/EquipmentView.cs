using System;

namespace ShoppingAreas.Services.Models
{
	public class EquipmentView
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public decimal Length { get; set; }
		public decimal Width { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
