using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Services;

namespace Nop.Data.Mapping.Services
{
  public  class CategoryMap : NopEntityTypeConfiguration<ServiceCategory>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            builder.ToTable(nameof(ServiceCategory));
            builder.HasKey(ServiceCategory => ServiceCategory.Id);
            builder.Property(ServiceCategory => ServiceCategory.Name).IsRequired();
            builder.Property(ServiceCategory => ServiceCategory.MetaKeywords).HasMaxLength(400);
            builder.Property(ServiceCategory => ServiceCategory.MetaTitle).HasMaxLength(400);
            base.Configure(builder);
        }

    }
}