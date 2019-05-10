using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShoppingAreas.Domain.Configurations;
using ShoppingAreas.Domain.Interceptors;
using ShoppingAreas.Domain.Models;

namespace ShoppingAreas.Domain
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Area> Areas { get; set; }

		private readonly CommandListener _adapter;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			_adapter = new CommandListener();

			var listener = this.GetService<DiagnosticSource>();
			(listener as DiagnosticListener).SubscribeWithAdapter(_adapter);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new AreaConfiguration());

			base.OnModelCreating(modelBuilder);
		}
	}
}
