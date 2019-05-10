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
				.Select(a => new AreaView
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
