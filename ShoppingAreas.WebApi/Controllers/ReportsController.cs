using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.WebApi.ViewModels;

namespace ShoppingAreas.WebApi.Controllers
{
	[Authorize(Policy = "ApiUser")]
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{
		private readonly IReportsService _reportsService;
		private readonly IMapper _mapper;

		public ReportsController(IReportsService reportsService, IMapper mapper)
		{
			_reportsService = reportsService;
			_mapper = mapper;
		}

		// GET: api/Reports
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmArea>>> GetReports(CancellationToken cancellationToken)
		{
			var reports = await _reportsService.GetReports(cancellationToken);
			var vmReports = _mapper.Map<IEnumerable<VmAreaReport>>(reports);

			return Ok(vmReports);
		}

		// GET: api/Reports/5
		[HttpGet("{id}")]
		public async Task<ActionResult<VmArea>> GetReport(Guid id, CancellationToken cancellationToken)
		{
			var areaReport = await _reportsService.GetReport(id, cancellationToken);
			if (areaReport == null)
			{
				return NotFound();
			}

			var vmReport = _mapper.Map<VmAreaReport>(areaReport);

			return Ok(vmReport);
		}
	}
}
