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
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public ProductsController(IProductService productService, IMapper mapper)
		{
			_productService = productService;
			_mapper = mapper;
		}

		// GET: api/Products
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VmProduct>>> GetProducts(string searchPattern, CancellationToken cancellationToken)
		{
			var productsQuery = _productService.GetProducts();
			if (!string.IsNullOrWhiteSpace(searchPattern))
			{
				productsQuery = productsQuery.Where(a => a.Name.Contains(searchPattern));
			}
				
			var products = await productsQuery.ToListAsync(cancellationToken);

			var vmProducts = _mapper.Map<IEnumerable<VmProduct>>(products);

			return Ok(vmProducts);
		}

		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<ActionResult<VmProduct>> GetProduct(Guid id, CancellationToken cancellationToken)
		{
			var product = await _productService.GetProducts(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (product == null)
			{
				return NotFound();
			}

			var vmProduct = _mapper.Map<VmProduct>(product);

			return Ok(vmProduct);
		}

		// PUT: api/Products/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProduct([FromRoute]Guid id, [FromBody]VmProduct vmProduct, CancellationToken cancellationToken)
		{
			if (id != vmProduct.Id)
			{
				return BadRequest();
			}

			var productView = _mapper.Map<ProductView>(vmProduct);
			await _productService.UpdateProductAsync(productView, cancellationToken);

			try
			{
				await _productService.CommitAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await ProductExists(id, cancellationToken))
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

		// POST: api/Products
		[HttpPost]
		public async Task<ActionResult<VmProduct>> PostProduct([FromBody]VmProduct vmProduct, CancellationToken cancellationToken)
		{
			var productView = _mapper.Map<ProductView>(vmProduct);
			var dbProduct = await _productService.AddProductAsync(productView, cancellationToken);
			await _productService.CommitAsync(cancellationToken);

			vmProduct.Id = dbProduct.Id;
			return CreatedAtAction("GetProduct", new { id = dbProduct.Id }, vmProduct);
		}

		// DELETE: api/Products/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<VmProduct>> DeleteProduct(Guid id, CancellationToken cancellationToken)
		{
			var product = await _productService.GetProducts(a => a.Id == id)
				.SingleOrDefaultAsync(cancellationToken);
			if (product == null)
			{
				return NotFound();
			}

			var productView = await _productService.DeleteProductAsync(id, cancellationToken);
			await _productService.CommitAsync(cancellationToken);

			var vmProduct = _mapper.Map<VmProduct>(product);

			return Ok(vmProduct);
		}

		private async Task<bool> ProductExists(Guid id, CancellationToken cancellationToken)
		{
			return await _productService.GetProducts()
				.AnyAsync(e => e.Id == id, cancellationToken);
		}
	}
}
