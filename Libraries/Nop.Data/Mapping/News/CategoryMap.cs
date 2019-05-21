using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
  public  class CategoryMap : NopEntityTypeConfiguration<NewsCategory>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            builder.ToTable(NopMappingDefaults.NewsCategoryTable);
            builder.HasKey(newsCategory => newsCategory.Id);
            builder.Property(newsCategory => newsCategory.Name).IsRequired();
            builder.Property(newsCategory => newsCategory.MetaKeywords).HasMaxLength(400);
            builder.Property(newsCategory => newsCategory.MetaTitle).HasMaxLength(400);
            base.Configure(builder);
        }

    }
}