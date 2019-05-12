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
	public class EquipmentService : IEquipmentService
	{
		private readonly ApplicationDbContext _context;

		public EquipmentService(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<EquipmentView> GetEquipments(Expression<Func<EquipmentView, bool>> expr = null)
		{
			var query = GetEquipmentsQuery();
			if (expr != null)
			{
				query = query.Where(expr);
			}

			return query;
		}

		public async Task<Equipment> AddEquipmentAsync(EquipmentView equipment, CancellationToken cancellationToken)
		{
			if (equipment == null) throw new ArgumentNullException(nameof(equipment));

			var dbEquipment = new Equipment
			{
				Id = Guid.NewGuid(),
				Name = equipment.Name,
				Length = equipment.Length,
				Width = equipment.Width,
				CreatedAt = DateTime.UtcNow
			};

			await _context.Equipments.AddAsync(dbEquipment, cancellationToken);

			return dbEquipment;
		}

		public async Task<Equipment> UpdateEquipmentAsync(EquipmentView equipment, CancellationToken cancellationToken)
		{
			if (equipment == null) throw new ArgumentNullException(nameof(equipment));

			var dbEquipment = await _context.Equipments
				.Where(h => h.Id == equipment.Id)
				.SingleOrDefaultAsync(cancellationToken);

			dbEquipment.Name = equipment.Name;
			dbEquipment.Length = equipment.Length;
			dbEquipment.Width = equipment.Width;
			dbEquipment.UpdatedAt = DateTime.UtcNow;

			_context.Entry(dbEquipment).State = EntityState.Modified;

			return dbEquipment;
		}

		public async Task<Equipment> DeleteEquipmentAsync(Guid id, CancellationToken cancellationToken)
		{
			var dbEquipment = await _context.Equipments
				.SingleOrDefaultAsync(h => h.Id == id, cancellationToken);

			if (dbEquipment == null) throw new ArgumentNullException(nameof(dbEquipment));

			dbEquipment.DeletedAt = DateTime.UtcNow;

			_context.Entry(dbEquipment).State = EntityState.Modified;

			return dbEquipment;
		}

		private IQueryable<EquipmentView> GetEquipmentsQuery()
		{
			var query = _context.Equipments
				.Where(a => a.DeletedAt == null)
				.Select(a => new EquipmentView
				{
					Id = a.Id,
					Name = a.Name,
					Length = a.Length,
					Width = a.Width,
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
