using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Services;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //specification attributes
        IConsumer<EntityInsertedEvent<SpecificationAttribute>>,
        IConsumer<EntityUpdatedEvent<SpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<SpecificationAttribute>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //newscategory
        IConsumer<EntityInsertedEvent<NewsCategory>>,
        IConsumer<EntityUpdatedEvent<NewsCategory>>,
        IConsumer<EntityDeletedEvent<NewsCategory>>,
         //servicecategory
         IConsumer<EntityInsertedEvent<ServiceCategory>>,
        IConsumer<EntityUpdatedEvent<ServiceCategory>>,
        IConsumer<EntityDeletedEvent<ServiceCategory>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public const string OFFICIAL_NEWS_MODEL_KEY = "Nop.pres.admin.official.news";
        public const string OFFICIAL_NEWS_PATTERN_KEY = "Nop.pres.admin.official.news";
        
        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public const string SPEC_ATTRIBUTES_MODEL_KEY = "Nop.pres.admin.product.specs";
        public const string SPEC_ATTRIBUTES_PATTERN_KEY = "Nop.pres.admin.product.specs";

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string CATEGORIES_LIST_KEY = "Nop.pres.admin.categories.list-{0}";
        public const string CATEGORIES_LIST_PATTERN_KEY = "Nop.pres.admin.categories.list";

        /// <summary>
        /// 
        /// </summary>
        public const string NEWSCATEGORIES_LIST_KEY = "Nop.pres.admin.news.categories.list-{0}";
        public const string NEWSCATEGORIES_LIST_PATTERN_KEY = "Nop.pres.admin.news.categories.list";

        public const string SERVICECATEGORIES_LIST_KEY = "Nop.pres.admin.service.categories.list-{0}";
        public const string SERVICECATEGORIES_LIST_PATTERN_KEY = "Nop.pres.admin.service.categories.list";
        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string MANUFACTURERS_LIST_KEY = "Nop.pres.admin.manufacturers.list-{0}";
        public const string MANUFACTURERS_LIST_PATTERN_KEY = "Nop.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string VENDORS_LIST_KEY = "Nop.pres.admin.vendors.list-{0}";
        public const string VENDORS_LIST_PATTERN_KEY = "Nop.pres.admin.vendors.list";

        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(OFFICIAL_NEWS_PATTERN_KEY); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }
        
        //specification attributes
        public void HandleEvent(EntityInsertedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        //newscategories
        public void HandleEvent(EntityInsertedEvent<NewsCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<NewsCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<NewsCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_LIST_PATTERN_KEY);
        }
        //servicecategories
        public void HandleEvent(EntityInsertedEvent<ServiceCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(SERVICECATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ServiceCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(SERVICECATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ServiceCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(SERVICECATEGORIES_LIST_PATTERN_KEY);
        }
        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }

        //vendors
        public void HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
    }
}