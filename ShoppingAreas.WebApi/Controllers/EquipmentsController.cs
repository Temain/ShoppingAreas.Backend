using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Services.Models;
using ShoppingAreas.WebApi.ViewModels;

namespace ShoppingAreas.WebApi.Controllers
{
	[Authorize(Policy = "ApiUser")]
	[Route("api/[controller]")]
	[ApiController]
	public class EquipmentsController : ControllerBase
	{
		private readonly IEquipmentService _equipmentService;
		private readonly IMapper _mapper;

		public EquipmentsController(IEquipmentService equipmentService, IMapper mapper)
		{
			_equipmentService = equipmentService;
			_mapper = mapper;
		}

		// GET: api/Equipments
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmEquipment>>> GetEquipments(string searchPattern, CancellationToken cancellationToken)
		{
			var equipmentsQuery = _equipmentService.GetEquipments();
			if (!string.IsNullOrWhiteSpace(searchPattern))
			{
				equipmentsQuery = equipmentsQuery.Where(a => a.Name.Contains(searchPattern));
			}
				
			var equipments = await equipmentsQuery.ToListAsync(cancellationToken);

			var vmEquipments = _mapper.Map<IEnumerable<VmEquipment>>(equipments);

			return Ok(vmEquipments);
		}

		// GET: api/Equipments/5
		[HttpGet("{id}")]
		public async Task<ActionResult<VmEquipment>> GetEquipment(Guid id, CancellationToken cancellationToken)
		{
			var equipment = await _equipmentService.GetEquipments(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (equipment == null)
			{
				return NotFound();
			}

			var vmEquipment = _mapper.Map<VmEquipment>(equipment);

			return Ok(vmEquipment);
		}

		// PUT: api/Equipments/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutEquipment([FromRoute]Guid id, [FromBody]VmEquipment vmEquipment, CancellationToken cancellationToken)
		{
			if (id != vmEquipment.Id)
			{
				return BadRequest();
			}

			var equipmentView = _mapper.Map<EquipmentView>(vmEquipment);
			await _equipmentService.UpdateEquipmentAsync(equipmentView, cancellationToken);

			try
			{
				await _equipmentService.CommitAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await EquipmentExists(id, cancellationToken))
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

		// POST: api/Equipments
		[HttpPost]
		public async Task<ActionResult<VmEquipment>> PostEquipment([FromBody]VmEquipment vmEquipment, CancellationToken cancellationToken)
		{
			var equipmentView = _mapper.Map<EquipmentView>(vmEquipment);
			var dbEquipment = await _equipmentService.AddEquipmentAsync(equipmentView, cancellationToken);
			await _equipmentService.CommitAsync(cancellationToken);

			vmEquipment.Id = dbEquipment.Id;
			return CreatedAtAction("GetEquipment", new { id = dbEquipment.Id }, vmEquipment);
		}

		// DELETE: api/Equipments/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<VmEquipment>> DeleteEquipment(Guid id, CancellationToken cancellationToken)
		{
			var equipment = await _equipmentService.GetEquipments(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (equipment == null)
			{
				return NotFound();
			}

			var equipmentView = await _equipmentService.DeleteEquipmentAsync(id, cancellationToken);
			await _equipmentService.CommitAsync(cancellationToken);

			var vmEquipment = _mapper.Map<VmEquipment>(equipment);

			return Ok(vmEquipment);
		}

		private async Task<bool> EquipmentExists(Guid id, CancellationToken cancellationToken)
		{
			return await _equipmentService.GetEquipments()
				.AnyAsync(e => e.Id == id, cancellationToken);
		}
	}
}
