using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services.Interfaces
{
	public interface IReportsService
	{
		Task<IEnumerable<AreaReportView>> GetReports(CancellationToken cancellationToken);
	}
}
