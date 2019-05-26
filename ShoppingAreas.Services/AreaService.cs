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
	public class AreaService : IAreaService
	{
		private readonly ApplicationDbContext _context;

		public AreaService(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<AreaView> GetAreas(Expression<Func<AreaView, bool>> expr = null)
		{
			var query = GetAreasQuery();
			if (expr != null)
			{
				query = query.Where(expr);
			}

			return query;
		}

		public async Task<Area> AddAreaAsync(AreaView area, CancellationToken cancellationToken)
		{
			if (area == null) throw new ArgumentNullException(nameof(area));

			var dbArea = new Area
			{
				Id = Guid.NewGuid(),
				Name = area.Name,
				TotalArea = area.TotalArea,
				Address = area.Address,
				ImagePath = area.ImagePath,
				ImageHash = area.ImageHash,
				ImageType = area.ImageType,
				CreatedAt = DateTime.UtcNow
			};

			await _context.Areas.AddAsync(dbArea, cancellationToken);

			return dbArea;
		}

		public async Task<Area> UpdateAreaAsync(AreaView area, CancellationToken cancellationToken)
		{
			if (area == null) throw new ArgumentNullException(nameof(area));

			var dbArea = await _context.Areas
				.Where(h => h.Id == area.Id)
				.SingleOrDefaultAsync(cancellationToken);

			dbArea.Name = area.Name;
			dbArea.Address = area.Address;
			dbArea.TotalArea = area.TotalArea;
			dbArea.ImagePath = area.ImagePath;
			dbArea.ImagePath = area.ImageHash;
			dbArea.ImageType = area.ImageType;
			dbArea.UpdatedAt = DateTime.UtcNow;

			_context.Entry(dbArea).State = EntityState.Modified;

			return dbArea;
		}

		public async Task<Area> DeleteAreaAsync(Guid id, CancellationToken cancellationToken)
		{
			var dbArea = await _context.Areas
				.SingleOrDefaultAsync(h => h.Id == id, cancellationToken);

			if (dbArea == null) throw new ArgumentNullException(nameof(dbArea));

			dbArea.DeletedAt = DateTime.UtcNow;

			_context.Entry(dbArea).State = EntityState.Modified;

			return dbArea;
		}

		private IQueryable<AreaView> GetAreasQuery()
		{
			var query = _context.Areas
				.Where(a => a.DeletedAt == null)
				.Select(a => new AreaView
				{
					Id = a.Id,
					Name = a.Name,
					Address = a.Address,
					TotalArea = a.TotalArea,
					ImagePath = a.ImagePath,
					ImageType = a.ImageType,
					ImageHash = a.ImageHash,
					CreatedAt = a.CreatedAt,
					UpdatedAt = a.UpdatedAt,
					DeletedAt = a.DeletedAt
				});

			return query;
		}

		public IQueryable<EquipmentAreaView> GetEquipmentAreas(Expression<Func<EquipmentAreaView, bool>> expr = null)
		{
			var query = GetEquipmentAreasQuery();
			if (expr != null)
			{
				query = query.Where(expr);
			}

			return query;
		}

		private IQueryable<EquipmentAreaView> GetEquipmentAreasQuery()
		{
			var query = _context.EquipmentAreas
				.Include(i => i.Equipment)
				.Where(a => a.DeletedAt == null)
				.Select(eq => new EquipmentAreaView
				{
					AreaId = eq.AreaId,
					EquipmentId = eq.EquipmentId,
					EquipmentName = eq.Equipment.Name,
					Length = eq.Equipment.Length,
					Width = eq.Equipment.Width,
					Count = eq.Count
				});

			return query;
		}

		public async Task<EquipmentArea> AddEquipmentAreaAsync(EquipmentAreaView equipArea, CancellationToken cancellationToken)
		{
			if (equipArea == null) throw new ArgumentNullException(nameof(equipArea));

			var dbEquipmentArea = new EquipmentArea
			{
				AreaId = equipArea.AreaId,
				EquipmentId = equipArea.EquipmentId,
				Count = equipArea.Count,
				CreatedAt = DateTime.UtcNow
			};

			await _context.EquipmentAreas.AddAsync(dbEquipmentArea, cancellationToken);

			return dbEquipmentArea;
		}

		public async Task<EquipmentArea> UpdateEquipmentAreaAsync(EquipmentAreaView equipArea, CancellationToken cancellationToken)
		{
			if (equipArea == null) throw new ArgumentNullException(nameof(equipArea));

			var dbEquipmentArea = await _context.EquipmentAreas
				.Where(eq => eq.AreaId == equipArea.AreaId && eq.EquipmentId == equipArea.EquipmentId)
				.SingleOrDefaultAsync(cancellationToken);

			dbEquipmentArea.Count = equipArea.Count;

			_context.Entry(dbEquipmentArea).State = EntityState.Modified;

			return dbEquipmentArea;
		}

		public async Task<EquipmentArea> DeleteEquipmentAreaAsync(Guid areaId, Guid equipmentId, CancellationToken cancellationToken)
		{
			var dbEquipmentArea = await _context.EquipmentAreas
				.SingleOrDefaultAsync(eq => eq.AreaId == areaId && eq.EquipmentId == equipmentId, cancellationToken);

			if (dbEquipmentArea == null) throw new ArgumentNullException(nameof(dbEquipmentArea));

			// dbEquipmentArea.DeletedAt = DateTime.UtcNow;

			// _context.Entry(dbEquipmentArea).State = EntityState.Modified;

			_context.EquipmentAreas.Remove(dbEquipmentArea);

			return dbEquipmentArea;
		}

		public IQueryable<ProductAreaView> GetProductsAreas(Expression<Func<ProductAreaView, bool>> expr = null)
		{
			var query = GetProductsAreasQuery();
			if (expr != null)
			{
				query = query.Where(expr);
			}

			return query;
		}

		private IQueryable<ProductAreaView> GetProductsAreasQuery()
		{
			var query = _context.ProductAreas
				.Include(i => i.Product)
				.Where(a => a.DeletedAt == null)
				.Select(eq => new ProductAreaView
				{
					AreaId = eq.AreaId,
					ProductId = eq.ProductId,
					ProductName = eq.Product.Name,
					Length = eq.Length,
					Width = eq.Width,
					Income = eq.Income,
					Profit = eq.Profit
				});

			return query;
		}

		public async Task<ProductArea> AddProductAreaAsync(ProductAreaView productArea, CancellationToken cancellationToken)
		{
			if (productArea == null) throw new ArgumentNullException(nameof(productArea));

			var dbProductArea = new ProductArea
			{
				AreaId = productArea.AreaId,
				ProductId = productArea.ProductId,
				Length = productArea.Length,
				Width = productArea.Width,
				Income = productArea.Income,
				Profit = productArea.Profit,
				CreatedAt = DateTime.UtcNow
			};

			await _context.ProductAreas.AddAsync(dbProductArea, cancellationToken);

			return dbProductArea;
		}

		public async Task<ProductArea> UpdateProductAreaAsync(ProductAreaView productArea, CancellationToken cancellationToken)
		{
			if (productArea == null) throw new ArgumentNullException(nameof(productArea));

			var dbProductArea = await _context.ProductAreas
				.Where(eq => eq.AreaId == productArea.AreaId && eq.ProductId == productArea.ProductId)
				.SingleOrDefaultAsync(cancellationToken);

			dbProductArea.Length = productArea.Length;
			dbProductArea.Width = productArea.Width;
			dbProductArea.Income = productArea.Income;
			dbProductArea.Profit = productArea.Profit;

			_context.Entry(dbProductArea).State = EntityState.Modified;

			return dbProductArea;
		}

		public async Task<ProductArea> DeleteProductAreaAsync(Guid areaId, Guid productId, CancellationToken cancellationToken)
		{
			var dbProductArea = await _context.ProductAreas
				.SingleOrDefaultAsync(eq => eq.AreaId == areaId && eq.ProductId == productId, cancellationToken);

			if (dbProductArea == null) throw new ArgumentNullException(nameof(dbProductArea));

			// dbProductArea.DeletedAt = DateTime.UtcNow;
			// _context.Entry(dbProductArea).State = EntityState.Modified;

			_context.ProductAreas.Remove(dbProductArea);

			return dbProductArea;
		}

		public Task<int> CommitAsync(CancellationToken cancellationToken)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}
	}
}
