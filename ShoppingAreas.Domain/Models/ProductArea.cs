using System;

namespace ShoppingAreas.Domain.Models
{
	public class ProductArea
	{
		public Guid ProductId { get; set; }
		public Product Product { get; set; }

		public Guid AreaId { get; set; }
		public Area Area { get; set; }

		public decimal Length { get; set; }
		public decimal Width { get; set; }

		public decimal Income { get; set; }
		public decimal Profit { get; set; }

		public int Year { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
		public DateTimeOffset? DeletedAt { get; set; }
	}
}
