using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Services;

namespace Nop.Data.Mapping.Services
{
    /// <summary>
    /// Represents a Service mapping configuration
    /// </summary>
    public partial class ServiceMap : NopEntityTypeConfiguration<Service>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable(nameof(Service));
            builder.HasKey(ServiceItem => ServiceItem.Id);
            builder.Property(ServiceItem => ServiceItem.Name).IsRequired();
            builder.Property(ServiceItem => ServiceItem.MetaKeywords).HasMaxLength(400);
            builder.Property(ServiceItem => ServiceItem.MetaTitle).HasMaxLength(400);
            base.Configure(builder);
        }

        #endregion
    }
}