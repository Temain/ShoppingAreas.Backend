using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services.Interfaces
{
	public interface IProductService
	{
		IQueryable<ProductView> GetProducts(Expression<Func<ProductView, bool>> expr = null);

		Task<Product> AddProductAsync(ProductView product, CancellationToken cancellationToken);

		Task<Product> UpdateProductAsync(ProductView product, CancellationToken cancellationToken);

		Task<Product> DeleteProductAsync(Guid id, CancellationToken cancellationToken);

		Task<int> CommitAsync(CancellationToken cancellationToken);
	}
}
