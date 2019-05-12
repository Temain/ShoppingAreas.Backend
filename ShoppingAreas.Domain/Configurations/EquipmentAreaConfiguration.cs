using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingAreas.Domain.Models;

namespace ShoppingAreas.Domain.Configurations
{
	public class EquipmentAreaConfiguration : IEntityTypeConfiguration<EquipmentArea>
	{
		public void Configure(EntityTypeBuilder<EquipmentArea> builder)
		{
			builder.ToTable("tx_equipment_area");

			builder.HasKey(p => new { p.AreaId, p.EquipmentId });

			builder.Property(p => p.AreaId).HasColumnName("id_area");
			builder.Property(p => p.EquipmentId).HasColumnName("id_equipment");
			builder.Property(p => p.Count).HasColumnName("count");
			builder.Property(p => p.Year).HasColumnName("year");
			builder.Property(p => p.CreatedAt).HasColumnName("created_at");
			builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
			builder.Property(p => p.DeletedAt).HasColumnName("deleted_at").IsRequired(false);
		}
	}
}
