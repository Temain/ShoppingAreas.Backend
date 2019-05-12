using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services.Models;

namespace ShoppingAreas.Services.Interfaces
{
	public interface IEquipmentService
	{
		IQueryable<EquipmentView> GetEquipments(Expression<Func<EquipmentView, bool>> expr = null);

		Task<Equipment> AddEquipmentAsync(EquipmentView equipment, CancellationToken cancellationToken);

		Task<Equipment> UpdateEquipmentAsync(EquipmentView equipment, CancellationToken cancellationToken);

		Task<Equipment> DeleteEquipmentAsync(Guid id, CancellationToken cancellationToken);

		Task<int> CommitAsync(CancellationToken cancellationToken);
	}
}
