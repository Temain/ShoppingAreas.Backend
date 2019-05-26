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
	public class ProductsAreaController : ControllerBase
	{
		private readonly IAreaService _areaService;
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public ProductsAreaController(IAreaService areaService, IProductService productService, IMapper mapper)
		{
			_areaService = areaService;
			_productService = productService;
			_mapper = mapper;
		}

		// GET: api/ProductsArea
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmProductArea>>> GetProducts(Guid areaId, bool all, CancellationToken cancellationToken)
		{
			var productsQuery = _areaService.GetProductsAreas(p => p.AreaId == areaId);
			if (all)
			{
				var productsIds = productsQuery.Select(p => p.ProductId);
				var productsAll = await _productService.GetProducts(eq => !productsIds.Contains(eq.Id))
					.ToListAsync(cancellationToken);

				var vmProducts = _mapper.Map<IEnumerable<VmProductArea>>(productsAll);
				foreach (var product in vmProducts)
				{
					product.AreaId = areaId;
				}

				return Ok(vmProducts);
			}
			else
			{
				var products = await productsQuery.ToListAsync(cancellationToken);
				var vmProducts = _mapper.Map<IEnumerable<VmProductArea>>(products);

				return Ok(vmProducts);
			}
		}

		// POST: api/ProductsArea
		[HttpPost]
		public async Task<ActionResult<VmProductArea>> PostProduct([FromBody]VmProductArea vmProduct, CancellationToken cancellationToken)
		{
			var productView = _mapper.Map<ProductAreaView>(vmProduct);
			var dbProduct = await _areaService.AddProductAreaAsync(productView, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			return CreatedAtAction("GetProducts", 
				new { areaId = dbProduct.AreaId, equipmentId = dbProduct.ProductId }, vmProduct);
		}

		// PUT: api/ProductsArea/5
		[HttpPut]
		public async Task<IActionResult> PutProduct(Guid areaId, Guid productId, [FromBody]VmProductArea vmProduct, CancellationToken cancellationToken)
		{
			if (areaId != vmProduct.AreaId && productId != vmProduct.ProductId)
			{
				return BadRequest();
			}

			var productView = _mapper.Map<ProductAreaView>(vmProduct);
			await _areaService.UpdateProductAreaAsync(productView, cancellationToken);

			try
			{
				await _areaService.CommitAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await ProductAreaExists(areaId, productId, cancellationToken))
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

		// DELETE: api/ProductsArea/5
		[HttpDelete]
		public async Task<ActionResult<VmProductArea>> DeleteProduct(Guid areaId, Guid productId, CancellationToken cancellationToken)
		{
			var product = await _areaService.GetProductsAreas(a => a.AreaId == areaId && a.ProductId == productId)
				.SingleOrDefaultAsync(cancellationToken);
			if (product == null)
			{
				return NotFound();
			}

			var productView = await _areaService.DeleteProductAreaAsync(areaId, productId, cancellationToken);
			await _areaService.CommitAsync(cancellationToken);

			var vmProduct = _mapper.Map<VmProductArea>(product);

			return Ok(vmProduct);
		}

		private async Task<bool> ProductAreaExists(Guid areaId, Guid productId, CancellationToken cancellationToken)
		{
			return await _areaService.GetProductsAreas()
				.AnyAsync(e => e.AreaId == areaId && e.ProductId == productId, cancellationToken);
		}
	}
}
