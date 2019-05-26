using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services.Interfaces
{
	public interface IAreaService
	{
		IQueryable<AreaView> GetAreas(Expression<Func<AreaView, bool>> expr = null);

		Task<Area> AddAreaAsync(AreaView area, CancellationToken cancellationToken);

		Task<Area> UpdateAreaAsync(AreaView area, CancellationToken cancellationToken);

		Task<Area> DeleteAreaAsync(Guid id, CancellationToken cancellationToken);

		IQueryable<EquipmentAreaView> GetEquipmentAreas(Expression<Func<EquipmentAreaView, bool>> expr = null);

		Task<EquipmentArea> AddEquipmentAreaAsync(EquipmentAreaView equipArea, CancellationToken cancellationToken);

		Task<EquipmentArea> UpdateEquipmentAreaAsync(EquipmentAreaView equipArea, CancellationToken cancellationToken);

		Task<EquipmentArea> DeleteEquipmentAreaAsync(Guid areaId, Guid equipmentId, CancellationToken cancellationToken);

		IQueryable<ProductAreaView> GetProductsAreas(Expression<Func<ProductAreaView, bool>> expr = null);

		Task<ProductArea> AddProductAreaAsync(ProductAreaView productArea, CancellationToken cancellationToken);

		Task<ProductArea> UpdateProductAreaAsync(ProductAreaView productArea, CancellationToken cancellationToken);

		Task<ProductArea> DeleteProductAreaAsync(Guid areaId, Guid productId, CancellationToken cancellationToken);

		Task<int> CommitAsync(CancellationToken cancellationToken);
	}
}
