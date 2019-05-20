using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services.Interfaces
{
	public interface IReportsService
	{
		Task<IEnumerable<AreaReportView>> GetAreaReports(CancellationToken cancellationToken);
		Task<AreaReportView> GetAreaReport(Guid id, CancellationToken cancellationToken);
		Task<IEnumerable<ProductReportView>> GetProductReports(Guid areaId, CancellationToken cancellationToken);
	}
}
