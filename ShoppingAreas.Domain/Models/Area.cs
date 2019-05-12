using System;
using System.Collections.Generic;

namespace ShoppingAreas.Domain.Models
{
	public class Area
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
