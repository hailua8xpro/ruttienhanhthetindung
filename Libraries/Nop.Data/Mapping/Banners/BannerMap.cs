using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Banners;
namespace Nop.Data.Mapping.Banners
{
    public partial class BannerMap : NopEntityTypeConfiguration<Banner>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable(nameof(Banner));
            builder.HasKey(banner => banner.Id);
            builder.Property(banner => banner.Title).IsRequired();
            builder.Property(banner => banner.PictureId).IsRequired();
            builder.Property(banner => banner.Type).IsRequired();
            builder.Ignore(banner => banner.BannerType);
            base.Configure(builder);
        }

        #endregion
    }
}
