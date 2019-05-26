using System;

namespace ShoppingAreas.Services.Models
{
	public class EquipmentAreaView
	{
		public Guid EquipmentId { get; set; }
		public string EquipmentName { get; set; }

		public decimal Length { get; set; }
		public decimal Width { get; set; }

		public Guid AreaId { get; set; }

		public int Count { get; set; }
	}
}
