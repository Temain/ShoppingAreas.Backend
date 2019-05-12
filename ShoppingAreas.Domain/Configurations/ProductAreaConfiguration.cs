using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingAreas.Domain.Models;

namespace ShoppingAreas.Domain.Configurations
{
	public class ProductAreaConfiguration : IEntityTypeConfiguration<ProductArea>
	{
		public void Configure(EntityTypeBuilder<ProductArea> builder)
		{
			builder.ToTable("tx_product_area");

			builder.HasKey(p => new { p.AreaId, p.ProductId });

			builder.Property(p => p.AreaId).HasColumnName("id_area");
			builder.Property(p => p.ProductId).HasColumnName("id_product");
			builder.Property(p => p.Length).HasColumnName("length");
			builder.Property(p => p.Width).HasColumnName("width");
			builder.Property(p => p.Income).HasColumnName("income");
			builder.Property(p => p.Profit).HasColumnName("profit");
			builder.Property(p => p.CreatedAt).HasColumnName("created_at");
			builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
			builder.Property(p => p.DeletedAt).HasColumnName("deleted_at").IsRequired(false);
		}
	}
}
