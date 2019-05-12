using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingAreas.Domain;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<ProductView> GetProducts(Expression<Func<ProductView, bool>> expr = null)
		{
			var query = GetProductsQuery();
			if (expr != null)
			{
				query = query.Where(expr);
			}

			return query;
		}

		public async Task<Product> AddProductAsync(ProductView product, CancellationToken cancellationToken)
		{
			if (product == null) throw new ArgumentNullException(nameof(product));

			var dbProduct = new Product
			{
				Id = Guid.NewGuid(),
				Name = product.Name,
				CreatedAt = DateTime.UtcNow
			};

			await _context.Products.AddAsync(dbProduct, cancellationToken);

			return dbProduct;
		}

		public async Task<Product> UpdateProductAsync(ProductView product, CancellationToken cancellationToken)
		{
			if (product == null) throw new ArgumentNullException(nameof(product));

			var dbProduct = await _context.Products
				.Where(h => h.Id == product.Id)
				.SingleOrDefaultAsync(cancellationToken);

			dbProduct.Name = product.Name;
			dbProduct.UpdatedAt = DateTime.UtcNow;

			_context.Entry(dbProduct).State = EntityState.Modified;

			return dbProduct;
		}

		public async Task<Product> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
		{
			var dbProduct = await _context.Products
				.SingleOrDefaultAsync(h => h.Id == id, cancellationToken);

			if (dbProduct == null) throw new ArgumentNullException(nameof(dbProduct));

			dbProduct.DeletedAt = DateTime.UtcNow;

			_context.Entry(dbProduct).State = EntityState.Modified;

			return dbProduct;
		}

		private IQueryable<ProductView> GetProductsQuery()
		{
			var query = _context.Products
				.Where(a => a.DeletedAt == null)
				.Select(a => new ProductView
				{
					Id = a.Id,
					Name = a.Name,
					CreatedAt = a.CreatedAt,
					UpdatedAt = a.UpdatedAt,
					DeletedAt = a.DeletedAt
				});

			return query;
		}

		public Task<int> CommitAsync(CancellationToken cancellationToken)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}
	}
}
