using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingAreas.WebApi.ViewModels
{
	public class VmEquipment
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
