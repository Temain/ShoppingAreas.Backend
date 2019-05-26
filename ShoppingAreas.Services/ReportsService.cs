using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingAreas.Domain;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services
{
	public class ReportsService : IReportsService
	{
		private readonly ApplicationDbContext _context;

		public ReportsService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<AreaReportView>> GetAreaReports(CancellationToken cancellationToken)
		{
			var areaReports = await GetAreaReportQuery(false)
				.ToListAsync(cancellationToken);

			foreach (var areaReport in areaReports)
			{
				areaReport.CoefInstall = areaReport.EquipmentArea / areaReport.TotalArea;
				areaReport.CoefDemo = areaReport.ProductArea / areaReport.TotalArea;
			}

			return areaReports;
		}

		public async Task<AreaReportView> GetAreaReport(Guid id, CancellationToken cancellationToken)
		{
			var areaReport = await GetAreaReportQuery(false)
				.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

			areaReport.CoefInstall = areaReport.EquipmentArea / areaReport.TotalArea;
			areaReport.CoefDemo = areaReport.ProductArea / areaReport.TotalArea;

			return areaReport;
		}

		private IQueryable<AreaReportView> GetAreaReportQuery(bool showDeleted)
		{
			return _context.Areas
				.Include(a => a.EquipmentAreas)
				.Include(a => a.ProductAreas)
				.Where(ar => showDeleted || ar.DeletedAt == null)
				.Select(a => new AreaReportView
				{
					Id = a.Id,
					Name = a.Name,
					Address = a.Address,
					TotalArea = a.TotalArea,
					EquipmentArea = a.EquipmentAreas
						.Sum(eq => eq.Equipment.Length * eq.Equipment.Width * eq.Count),
					ProductArea = a.ProductAreas
						.Sum(pa => pa.Length * pa.Width)
				});
		}

		public async Task<IEnumerable<ProductReportView>> GetProductReports(Guid areaId, CancellationToken cancellationToken)
		{
			var productReports = await GetProductReportQuery()
				.Where(a => a.AreaId == areaId)
				.ToListAsync(cancellationToken);

			var productsArea = productReports.Sum(p => p.Length * p.Width);
			var totalIncome = productReports.Sum(p => p.Income);
			var totalProfit = productReports.Sum(p => p.Profit);
			foreach (var product in productReports)
			{
				product.AreaPercent = Math.Round((product.Length * product.Width) / productsArea * 100, 2);
				product.IncomePercent = Math.Round(product.Income / totalIncome * 100, 2);
				product.ProfitPercent = Math.Round(product.Profit / totalProfit * 100, 2);
				product.CoefIncome = Math.Round(product.IncomePercent / product.AreaPercent, 2);
				product.CoefProfit = Math.Round(product.ProfitPercent / product.AreaPercent, 2);
			}

			return productReports;
		}

		private IQueryable<ProductReportView> GetProductReportQuery()
		{
			return _context.ProductAreas
				.Include(p => p.Product)
				.Include(p => p.Area)
				.Select(p => new ProductReportView
				{
					Id = p.ProductId,
					Name = p.Product.Name,
					AreaId = p.AreaId,
					TotalArea = p.Area.TotalArea,
					Length = p.Length,
					Width = p.Width,
					Income = p.Income,
					Profit = p.Profit
				});
		}
	}
}
