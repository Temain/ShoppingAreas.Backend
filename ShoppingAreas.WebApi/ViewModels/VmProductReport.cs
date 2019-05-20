using System;

namespace ShoppingAreas.WebApi.ViewModels
{
	public class VmProductReport
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public Guid AreaId { get; set; }
		public decimal TotalArea { get; set; }
		public decimal AreaPercent { get; set; }

		public decimal Length { get; set; }
		public decimal Width { get; set; }

		public decimal Income { get; set; }
		public decimal IncomePercent { get; set; }

		public decimal Profit { get; set; }
		public decimal ProfitPercent { get; set; }

		public decimal CoefProfit { get; set; }
		public decimal CoefIncome { get; set; }
	}
}
