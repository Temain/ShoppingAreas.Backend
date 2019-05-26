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
	public class EquipmentsAreaController : ControllerBase
	{
		private readonly IAreaService _areaService;
		private readonly IEquipmentService _equipmentService;
		private readonly IMapper _mapper;

		public EquipmentsAreaController(IAreaService areaService, IEquipmentService equipmentService
			, IMapper mapper)
		{
			_areaService = areaService;
			_equipmentService = equipmentService;
			_mapper = mapper;
		}

		// GET: api/EquipmentsArea
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmEquipmentArea>>> GetEquipments(Guid areaId, bool all, CancellationToken cancellationToken)
		{
			var equipmentsQuery = _areaService.GetEquipmentAreas(eq => eq.AreaId == areaId);
			if (all)
			{
				var equipmentsIds = equipmentsQuery.Select(eq => eq.EquipmentId);
				var equipmentsAll = await _equipmentService.GetEquipments(eq => !equipmentsIds.Contains(eq.Id))
					.ToListAsync(cancellationToken);

				var vmEquipments = _mapper.Map<IEnumerable<VmEquipmentArea>>(equipmentsAll);
				foreach (var equipment in vmEquipments)
				{
					equipment.AreaId = areaId;
				}

				return Ok(vmEquipments);
			}
			else
			{
				var equipments = await equipmentsQuery.ToListAsync(cancellationToken);
				var vmEquipments = _mapper.Map<IEnumerable<VmEquipmentArea>>(equipments);

				return Ok(vmEquipments);
			}
		}

		// POST: api/EquipmentsArea
		[HttpPost]
		public async Task<ActionResult<VmEquipmentArea>> PostEquipment([FromBody]VmEquipmentArea vmEquipment, CancellationToken cancellationToken)
		{
			var equipmentView = _mapper.Map<EquipmentAreaView>(vmEquipment);
			var dbEquipment = await _areaService.AddEquipmentAreaAsync(equipmentView, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			return CreatedAtAction("GetEquipments", 
				new { areaId = dbEquipment.AreaId, equipmentId = dbEquipment.EquipmentId, }, vmEquipment);
		}

		// PUT: api/EquipmentsArea/5
		[HttpPut]
		public async Task<IActionResult> PutEquipment(Guid areaId, Guid equipmentId, [FromBody]VmEquipmentArea vmEquipment, CancellationToken cancellationToken)
		{
			if (areaId != vmEquipment.AreaId && equipmentId != vmEquipment.EquipmentId)
			{
				return BadRequest();
			}

			var equipmentView = _mapper.Map<EquipmentAreaView>(vmEquipment);
			await _areaService.UpdateEquipmentAreaAsync(equipmentView, cancellationToken);

			try
			{
				await _areaService.CommitAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await EquipmentAreaExists(areaId, equipmentId, cancellationToken))
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

		// DELETE: api/EquipmentsArea/5
		[HttpDelete]
		public async Task<ActionResult<VmEquipmentArea>> DeleteEquipment(Guid areaId, Guid equipmentId, CancellationToken cancellationToken)
		{
			var equipment = await _areaService.GetEquipmentAreas(a => a.AreaId == areaId && a.EquipmentId == equipmentId)
				.SingleOrDefaultAsync(cancellationToken);
			if (equipment == null)
			{
				return NotFound();
			}

			var equipmentView = await _areaService.DeleteEquipmentAreaAsync(areaId, equipmentId, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			var vmEquipment = _mapper.Map<VmEquipmentArea>(equipment);

			return Ok(vmEquipment);
		}

		private async Task<bool> EquipmentAreaExists(Guid areaId, Guid equipmentId, CancellationToken cancellationToken)
		{
			return await _areaService.GetEquipmentAreas()
				.AnyAsync(e => e.AreaId == areaId && e.EquipmentId == equipmentId, cancellationToken);
		}
	}
}
