using System;

namespace ShoppingAreas.Domain.Models
{
	public class EquipmentArea
	{
		public Guid EquipmentId { get; set; }
		public Equipment Equipment { get; set; }

		public Guid AreaId { get; set; }
		public Area Area { get; set; }

		public int Count { get; set; }
		public int Year { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
