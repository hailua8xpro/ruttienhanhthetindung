using Nop.Core.Domain.Testimonial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Nop.Data.Mapping.Testimonials
{
   public class TestimonialMap : NopEntityTypeConfiguration<Testimonial>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.ToTable(nameof(Testimonial));
            builder.HasKey(testimonial => testimonial.Id);
            builder.Property(testimonial => testimonial.Description).IsRequired();
            builder.Property(testimonial => testimonial.FullDescription).IsRequired();
            builder.Property(testimonial => testimonial.FullName).IsRequired();
            base.Configure(builder);
        }

        #endregion
    }
}
