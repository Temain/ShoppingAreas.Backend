using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingAreas.Domain.Models
{
	public class Equipment
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
