using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Services.Models;
using ShoppingAreas.WebApi.ViewModels;

namespace ShoppingAreas.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AreasController : ControllerBase
	{
		private readonly IAreaService _areaService;
		private readonly IMapper _mapper;

		public AreasController(IAreaService areaService, IMapper mapper)
		{
			_areaService = areaService;
			_mapper = mapper;
		}

		// GET: api/Areas
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmArea>>> GetAreas(string searchPattern, CancellationToken cancellationToken)
		{
			var areasQuery = _areaService.GetAreas();
			if (!string.IsNullOrWhiteSpace(searchPattern))
			{
				areasQuery = areasQuery.Where(a => a.Name.Contains(searchPattern));
			}
				
			var areas = await areasQuery.ToListAsync(cancellationToken);

			var vmAreas = _mapper.Map<IEnumerable<VmArea>>(areas);

			return Ok(vmAreas);
		}

		// GET: api/Areas/5
		[HttpGet("{id}")]
		public async Task<ActionResult<VmArea>> GetArea(Guid id, CancellationToken cancellationToken)
		{
			var area = await _areaService.GetAreas(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (area == null)
			{
				return NotFound();
			}

			var vmArea = _mapper.Map<VmArea>(area);

			return Ok(vmArea);
		}

		// PUT: api/Areas/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutArea([FromRoute]Guid id, [FromBody]VmArea vmArea, CancellationToken cancellationToken)
		{
			if (id != vmArea.Id)
			{
				return BadRequest();
			}

			var areaView = _mapper.Map<AreaView>(vmArea);
			await _areaService.UpdateAreaAsync(areaView, cancellationToken);

			try
			{
				await _areaService.CommitAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await AreaExists(id, cancellationToken))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Areas
		[HttpPost]
		public async Task<ActionResult<VmArea>> PostArea([FromBody]VmArea vmArea, CancellationToken cancellationToken)
		{
			var areaView = _mapper.Map<AreaView>(vmArea);
			var dbArea = await _areaService.AddAreaAsync(areaView, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			vmArea.Id = dbArea.Id;
			return CreatedAtAction("GetArea", new { id = dbArea.Id }, vmArea);
		}

		// DELETE: api/Areas/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<VmArea>> DeleteArea(Guid id, CancellationToken cancellationToken)
		{
			var area = await _areaService.GetAreas(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (area == null)
			{
				return NotFound();
			}

			var areaView = await _areaService.DeleteAreaAsync(id, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			var vmArea = _mapper.Map<VmArea>(area);

			return Ok(vmArea);
		}

		private async Task<bool> AreaExists(Guid id, CancellationToken cancellationToken)
		{
			return await _areaService.GetAreas()
				.AnyAsync(e => e.Id == id, cancellationToken);
		}
	}
}
