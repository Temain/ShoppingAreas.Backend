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

		// GET: api/Reports/Area
		[HttpGet]
		[Route("Area")]
		public async Task<ActionResult<IEnumerable<VmArea>>> GetAreaReports(CancellationToken cancellationToken)
		{
			var reports = await _reportsService.GetAreaReports(cancellationToken);
			var vmReports = _mapper.Map<IEnumerable<VmAreaReport>>(reports);

			return Ok(vmReports);
		}

		// GET: api/Reports/Area/5
		[HttpGet("Area/{id}")]
		public async Task<ActionResult<VmArea>> GetAreaReport(Guid id, CancellationToken cancellationToken)
		{
			var areaReport = await _reportsService.GetAreaReport(id, cancellationToken);
			if (areaReport == null)
			{
				return NotFound();
			}

			var vmReport = _mapper.Map<VmAreaReport>(areaReport);

			return Ok(vmReport);
		}

		// GET: api/Reports/Product/5
		[HttpGet("Product/{id}")]
		public async Task<ActionResult<VmArea>> GetProductReport(Guid id, CancellationToken cancellationToken)
		{
			var productReports = await _reportsService.GetProductReports(id, cancellationToken);
			var vmReports = _mapper.Map<IEnumerable<VmProductReport>>(productReports);

			return Ok(vmReports);
		}
	}
}
