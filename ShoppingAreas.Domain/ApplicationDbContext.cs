using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShoppingAreas.Domain.Interceptors;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Helpers;

namespace ShoppingAreas.Domain
{
	public class ApplicationDbContext : IdentityDbContext<User, Role, long>
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
			// modelBuilder.ApplyConfiguration(new AreaConfiguration());

			base.OnModelCreating(modelBuilder);

			BuildIdentityTables(modelBuilder);
		}

		private void BuildIdentityTables(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().ToTable("asp_user");
			modelBuilder.Entity<IdentityRole<long>>().ToTable("asp_role");
			modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("asp_user_claim");
			modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("asp_user_login");
			modelBuilder.Entity<IdentityUserRole<long>>().ToTable("asp_user_role");
			modelBuilder.Entity<IdentityUserToken<long>>().ToTable("asp_user_token");
			modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("asp_role_claim");

			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				// Replace table names
				entity.Relational().TableName = entity.Relational().TableName.ToSnakeCase();

				// Replace column names            
				foreach (var property in entity.GetProperties())
				{
					property.Relational().ColumnName = property.Name.ToSnakeCase();
				}

				foreach (var key in entity.GetKeys())
				{
					key.Relational().Name = key.Relational().Name.ToSnakeCase();
				}

				foreach (var key in entity.GetForeignKeys())
				{
					key.Relational().Name = key.Relational().Name.ToSnakeCase();
				}

				foreach (var index in entity.GetIndexes())
				{
					index.Relational().Name = index.Relational().Name.ToSnakeCase();
				}
			}
		}
	}
}
