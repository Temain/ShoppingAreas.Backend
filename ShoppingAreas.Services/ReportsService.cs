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

		public async Task<IEnumerable<AreaReportView>> GetReports(CancellationToken cancellationToken)
		{
			var areaReports = await GetAreaReportQuery()
				.ToListAsync(cancellationToken);

			foreach (var areaReport in areaReports)
			{
				areaReport.CoefInstall = areaReport.EquipmentArea / areaReport.TotalArea;
				areaReport.CoefDemo = areaReport.ProductArea / areaReport.TotalArea;
			}

			return areaReports;
		}

		public async Task<AreaReportView> GetReport(Guid id, CancellationToken cancellationToken)
		{
			var areaReport = await GetAreaReportQuery()
				.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

			areaReport.CoefInstall = areaReport.EquipmentArea / areaReport.TotalArea;
			areaReport.CoefDemo = areaReport.ProductArea / areaReport.TotalArea;

			return areaReport;
		}

		private IQueryable<AreaReportView> GetAreaReportQuery()
		{
			return _context.Areas
				.Include(a => a.EquipmentAreas/*.Select(ea => ea.Equipment)*/)
				.Include(a => a.ProductAreas/*.Select(pa => pa.Product)*/)
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
	}
}
