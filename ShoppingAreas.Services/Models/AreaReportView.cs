using System;

namespace ShoppingAreas.Services.Models
{
	public class AreaReportView
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }

		/// <summary>
		/// Общая площадь торгового зала
		/// </summary>
		public decimal TotalArea { get; set; }

		/// <summary>
		/// Площадь занятая оборудованием
		/// </summary>
		public decimal EquipmentArea { get; set; }

		/// <summary>
		/// Площадь выкладки товаров
		/// </summary>
		public decimal ProductArea { get; set; }

		/// <summary>
		/// Коэффициент установочной площади
		/// </summary>
		public decimal CoefInstall { get; set; }

		/// <summary>
		/// Коэффициент демонстрационной площади
		/// </summary>
		public decimal CoefDemo { get; set; }
	}
}
