using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Services
{
    /// <summary>
    /// Represents a Service item
    /// </summary>
    public partial class Service : BaseEntity, ILocalizedEntity, ISlugSupported, IStoreMappingSupported
    {
        private ICollection<ServiceCategoryMapping> _serviceCategories;

        /// <summary>
        /// Gets or sets the Service title
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the Service image thumbnail
        /// </summary>
        public int PictureId { get; set; }
        /// <summary>
        /// Gets or sets the short text
        /// </summary>
        public string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text
        /// </summary>
        public string Full { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Service item is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }
        public bool IncludeInTopMenu { get; set; }
        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<ServiceCategoryMapping> ServiceCategories
        {
            get => _serviceCategories ?? (_serviceCategories = new List<ServiceCategoryMapping>());
            protected set => _serviceCategories = value;
        }
    }
}