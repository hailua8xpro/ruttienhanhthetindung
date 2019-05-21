using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Services;

namespace Nop.Data.Mapping.Services
{
    public partial class ServiceCategoryMap : NopEntityTypeConfiguration<ServiceCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ServiceCategoryMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.ServiceCategoryMappingTable);
            builder.HasKey(ServiceCategory => ServiceCategory.Id);

            builder.HasOne(ServiceCategory => ServiceCategory.Category)
                .WithMany()
                .HasForeignKey(ServiceCategory => ServiceCategory.CategoryId)
                .IsRequired();

            builder.HasOne(ServiceCategory => ServiceCategory.Service)
                .WithMany(Serviceitem => Serviceitem.ServiceCategories)
                .HasForeignKey(ServiceCategory => ServiceCategory.ServiceId)
                .IsRequired();
            base.Configure(builder);
        }

        #endregion
    }
}
