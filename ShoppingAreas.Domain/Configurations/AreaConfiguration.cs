using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingAreas.Domain.Models;

namespace ShoppingAreas.Domain.Configurations
{
	public class AreaConfiguration : IEntityTypeConfiguration<Area>
	{
		public void Configure(EntityTypeBuilder<Area> builder)
		{
			builder.ToTable("areas");

			builder.HasKey(p => p.Id);

			builder.Property(p => p.Id).HasColumnName("id");
			builder.Property(p => p.Name).HasColumnName("name");
			builder.Property(p => p.CreatedAt).HasColumnName("created_at");
			builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
			builder.Property(p => p.DeletedAt).HasColumnName("deleted_at").IsRequired(false);
		}
	}
}
