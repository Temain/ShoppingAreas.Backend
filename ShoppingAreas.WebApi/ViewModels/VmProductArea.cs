using System;

namespace ShoppingAreas.WebApi.ViewModels
{
	public class VmProductArea
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }

		public Guid AreaId { get; set; }

		public decimal Length { get; set; }
		public decimal Width { get; set; }

		public decimal Income { get; set; }
		public decimal Profit { get; set; }
	}
}
