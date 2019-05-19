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
		private readonly IAreaService _areaService;

		public ReportsService(ApplicationDbContext context, IAreaService areaService)
		{
			_context = context;
			_areaService = areaService;
		}

		public async Task<IEnumerable<AreaReportView>> GetReports(CancellationToken cancellationToken)
		{
			var areaQuery = _areaService.GetAreas();
			var areaIdsQuery = areaQuery.Select(a => a.Id);
			var areaReports = await areaQuery
				.Select(a => new AreaReportView
				{
					Id = a.Id,
					Name = a.Name,
					Address = a.Address,
					TotalArea = a.TotalArea
				})
				.ToListAsync(cancellationToken);

			var eqw = _context.EquipmentAreas.Where(eq => true);
			var eqw1 = await eqw.ToListAsync(cancellationToken);

			var equipmentArea = await _context.EquipmentAreas
				.Include(eq => eq.Equipment)
				.Include(eq => eq.Area)
				.Where(eq => areaIdsQuery.Contains(eq.AreaId))
				.GroupBy(g => g.AreaId)
				.ToDictionaryAsync(eq => eq.Key
					, v => v.Sum(i => i.Equipment.Length * i.Equipment.Width * i.Count));

			var pr = _context.ProductAreas.Where(eq => true);
			var pr1 = await pr.ToListAsync(cancellationToken);

			var productArea = await _context.ProductAreas
				.Include(eq => eq.Product)
				.Include(eq => eq.Area)
				.Where(pa => areaIdsQuery.Contains(pa.AreaId))
				.GroupBy(g => g.AreaId)
				.ToDictionaryAsync(eq => eq.Key
					, v => v.Sum(i => i.Length * i.Width));

			foreach (var areaReport in areaReports)
			{
				if (equipmentArea.ContainsKey(areaReport.Id))
				{
					areaReport.EquipmentArea = equipmentArea[areaReport.Id];
				}

				if (productArea.ContainsKey(areaReport.Id))
				{
					areaReport.ProductArea = productArea[areaReport.Id];
				}

				areaReport.CoefInstall = areaReport.EquipmentArea / areaReport.TotalArea;
				areaReport.CoefDemo = areaReport.ProductArea / areaReport.TotalArea;
			}

			return areaReports;
		}
	}
}
