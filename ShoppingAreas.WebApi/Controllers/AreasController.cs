using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Helpers;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Services.Models;
using ShoppingAreas.Web.Services;
using ShoppingAreas.Web.ViewModels;
using ShoppingAreas.WebApi.ViewModels;

namespace ShoppingAreas.WebApi.Controllers
{
	[Authorize(Policy = "ApiUser")]
	[Route("api/[controller]")]
	[ApiController]
	public class AreasController : ControllerBase
	{
		private readonly IAreaService _areaService;
		private readonly IImageService _imageService; 
		private readonly IMapper _mapper;

		public AreasController(IAreaService areaService, IImageService imageService, IMapper mapper)
		{
			_areaService = areaService;
			_imageService = imageService;
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

			// Изображения
			try
			{
				ParallelOptions parallelOptions = new ParallelOptions();
				parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
				Parallel.ForEach(vmAreas, parallelOptions, async vmArea =>
				{
					var imagePath = vmArea.ImagePath;
					var imageType = vmArea.ImageType;
					vmArea.Image = await GetImage(imagePath, imageType, cancellationToken);
				});
			}
			catch { }	

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

			var imagePath = vmArea.ImagePath;
			var imageType = vmArea.ImageType;
			vmArea.Image = await GetImage(imagePath, imageType, cancellationToken);

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
			var area = await _areaService.UpdateAreaAsync(areaView, cancellationToken);

			// Загрузка изображения
			var vmImage = vmArea.Image;
			var imageHash = areaView.ImageHash;
			await UploadImage(area, vmImage, imageHash, cancellationToken);

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

			// Загрузка изображения
			var vmImage = vmArea.Image;
			var imageHash = areaView.ImageHash;
			await UploadImage(dbArea, vmImage, imageHash, cancellationToken);

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

		private async Task<VmFile> GetImage(string imagePath, string imageType, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrWhiteSpace(imagePath))
			{
				var image = await _imageService.GetImageAsync(imagePath, cancellationToken);
				return new VmFile
				{
					FileType = imageType,
					FileData = $"data:{imageType};base64,{image}"
				};
			}

			return null;
		}

		private async Task UploadImage(Area area, VmFile vmImage, string imageHash, CancellationToken cancellationToken)
		{
			// Загрузка изображения
			if (vmImage != null && !string.IsNullOrWhiteSpace(vmImage.FileData))
			{
				var newImage = vmImage.FileData;
				var newImageHash = Md5Hash.GetHashString(newImage);

				if (imageHash != newImageHash)
				{
					var extention = Path.GetExtension(vmImage.FileName);
					var imagePath = $"/images/areas/{area.Id}{extention}";
					await _imageService.UploadImageAsync(imagePath, newImage, cancellationToken);
					area.ImagePath = imagePath;
					area.ImageType = vmImage.FileType;
					area.ImageHash = newImageHash;
				}
			}
		}
	}
}
