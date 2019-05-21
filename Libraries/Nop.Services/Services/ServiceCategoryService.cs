using System;
using System.Linq;

using System.Collections.Generic;
using Nop.Core.Caching;
using Nop.Services.Events;
using Nop.Core;
using Nop.Core.Data;

using Nop.Core.Domain.Services;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Localization;

namespace Nop.Services.Services
{
    public partial class ServiceCategoryService : IServiceCategoryService
    {
        #region Fields
        private readonly Core.Domain.Catalog.CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ServiceCategory> _categoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly string _entityName;
        private readonly IRepository<ServiceCategoryMapping> _ServiceCategoryMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Service> _ServiceRepository;
        private readonly ILocalizationService _localizationService;

        #endregion
        #region Ctor
        public ServiceCategoryService(
            Core.Domain.Catalog.CatalogSettings catalogSettings,
            IAclService aclService,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            IStaticCacheManager staticCacheManager,
            IRepository<ServiceCategory> categoryRepository,
            IRepository<AclRecord>  aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IRepository<ServiceCategoryMapping> ServiceCategoryRepository,
            IRepository<Service> ServiceRepository,
            IStoreMappingService storeMappingService
            )
        {
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._staticCacheManager = staticCacheManager;
            this._categoryRepository = categoryRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._aclRepository = aclRepository;
            this._entityName = typeof(ServiceCategory).Name;
            this._ServiceCategoryMappingRepository = ServiceCategoryRepository;
            this._storeMappingService = storeMappingService;
            this._ServiceRepository = ServiceRepository;
            this._localizationService = localizationService;
        }
        #endregion
        #region Method
        public void DeleteServiceCategory(ServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            category.Deleted = true;
            UpdateServiceCategory(category);

            //event notification
            _eventPublisher.EntityDeleted(category);

            //reset a "Parent category" property of all child subcategories
            var subcategories = GetAllServiceCategoriesByParentCategoryId(category.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateServiceCategory(subcategory);
            }
        }

        public void DeleteServiceCategoryMapping(ServiceCategoryMapping ServiceCategory)
        {
            throw new NotImplementedException();
        }

        public ServiceCategoryMapping FindServiceCategoryMapping(IList<ServiceCategoryMapping> source, int ServiceId, int categoryId)
        {
            foreach (var ServiceCategoryMapping in source)
                if (ServiceCategoryMapping.ServiceId == ServiceId && ServiceCategoryMapping.CategoryId == categoryId)
                    return ServiceCategoryMapping;

            return null;
        }

        public IList<ServiceCategory> GetAllServiceCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true)
        {
            IList<ServiceCategory> LoadCategoriesFunc() => GetAllServiceCategories(string.Empty, storeId, showHidden: showHidden);

            IList<ServiceCategory> categories;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(NopServiceDefaults.CategoriesAllCacheKey,
                    storeId,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    showHidden);
                categories = _staticCacheManager.Get(key, () =>
                {
                    var result = new List<ServiceCategory>();
                    foreach (var category in LoadCategoriesFunc())
                        result.Add(new CategoryForCaching(category));
                    return result;
                });
            }
            else
            {
                categories = LoadCategoriesFunc();
            }

            return categories;
        }

        public IPagedList<ServiceCategory> GetAllServiceCategories(string categoryName, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = this.SortServiceCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<ServiceCategory>(sortedCategories, pageIndex, pageSize);
        }

        public IList<ServiceCategory> GetAllServiceCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false)
        {
            var key = string.Format(NopServiceDefaults.CategoriesByParentCategoryIdCacheKey, parentCategoryId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select c;
                    }

                    query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var categories = query.ToList();
                return categories;
            });
        }

        public ServiceCategory GetServiceCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            var key = string.Format(NopServiceDefaults.CategoriesByIdCacheKey, categoryId);
            return _cacheManager.Get(key, () => _categoryRepository.GetById(categoryId));
        }

        public IList<int> GetChildCategoryIds(int parentCategoryId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = string.Format(NopServiceDefaults.CategoriesChildIdentifiersCacheKey,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                showHidden);
            return _staticCacheManager.Get(cacheKey, () =>
            {
                //little hack for performance optimization
                //there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
                //so we load all categories at once (we know they are cached) and process them server-side
                var categoriesIds = new List<int>();
                var categories = GetAllServiceCategories(storeId: storeId, showHidden: showHidden)
                    .Where(c => c.ParentCategoryId == parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id, storeId, showHidden));
                }

                return categoriesIds;
            });
        }
        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public IPagedList<ServiceCategoryMapping> GetServiceCategoryMappingByCategoryId(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<ServiceCategoryMapping>(new List<ServiceCategoryMapping>(), pageIndex, pageSize);

            var key = string.Format(NopServiceDefaults.ServiceCategoriesAllByCategoryIdCacheKey, showHidden, categoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from nc in _ServiceCategoryMappingRepository.Table
                            join p in _ServiceRepository.Table on nc.ServiceId equals p.Id
                            where nc.CategoryId == categoryId &&
                                  (showHidden || p.Published)
                            orderby nc.Id
                            select nc;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pc in query
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pc;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pc in query
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select pc;
                    }

                    query = query.Distinct().OrderByDescending(pc => pc.Id).ThenBy(pc => pc.Id);
                }

                var ServiceCategories = new PagedList<ServiceCategoryMapping>(query, pageIndex, pageSize);
                return ServiceCategories;
            });
        }
        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="ServiceId">Product identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public IList<ServiceCategoryMapping> GetServiceCategoryMappingByServiceId(int ServiceId, bool showHidden = false)
        {
            return GetServiceCategoryMappingByServiceId(ServiceId, _storeContext.CurrentStore.Id, showHidden);
        }
        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="ServiceId">Service identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Service category mapping collection</returns>
        public IList<ServiceCategoryMapping> GetServiceCategoryMappingByServiceId(int ServiceId, int storeId, bool showHidden = false)
        {
            if (ServiceId == 0)
                return new List<ServiceCategoryMapping>();

            var key = string.Format(NopServiceDefaults.ServiceCategoriesAllByServiceIdCacheKey, showHidden, ServiceId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _ServiceCategoryMappingRepository.Table
                            join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                            where pc.ServiceId == ServiceId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.Id descending
                            select pc;

                var allServiceCategories = query.ToList();
                var result = new List<ServiceCategoryMapping>();
                if (!showHidden)
                {
                    foreach (var pc in allServiceCategories)
                    {
                        //ACL (access control list) and store mapping
                        var category = pc.Category;
                        if (_aclService.Authorize(category) && _storeMappingService.Authorize(category, storeId))
                            result.Add(pc);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allServiceCategories);
                }

                return result;
            });
        }
        /// <summary>
        /// Gets a Service category mapping 
        /// </summary>
        /// <param name="ServiceCategoryId">Service category mapping identifier</param>
        /// <returns>Service category mapping</returns>
        public ServiceCategoryMapping GetServiceCategoryMappingById(int ServiceCategoryId)
        {
            if (ServiceCategoryId == 0)
                return null;

            return _ServiceCategoryMappingRepository.GetById(ServiceCategoryId);
        }
        /// <summary>
        /// Get category IDs for Service
        /// </summary>
        /// <param name="ServiceIds">Service IDs</param>
        /// <returns>Category IDs for Service</returns>
        public IDictionary<int, int[]> GetServiceCategoryIds(int[] ServiceIds)
        {
            var query = _ServiceCategoryMappingRepository.Table;

            return query.Where(p => ServiceIds.Contains(p.ServiceId))
                .Select(p => new { p.ServiceId, p.CategoryId }).ToList()
                .GroupBy(a => a.ServiceId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.CategoryId).ToArray());
        }
        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        public string[] GetNotExistingServiceCategories(string[] categoryIdsNames)
        {
            if (categoryIdsNames == null)
                throw new ArgumentNullException(nameof(categoryIdsNames));

            var query = _categoryRepository.Table;
            var queryFilter = categoryIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = query.Select(c => c.Id.ToString()).Where(c => queryFilter.Contains(c)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            return queryFilter.ToArray();
        }
        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public void InsertServiceCategory(ServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _categoryRepository.Insert(category);

            //cache
            _cacheManager.RemoveByPattern(NopServiceDefaults.CategoriesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopServiceDefaults.CategoriesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopServiceDefaults.ServiceCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(category);
        }
        /// <summary>
        /// Inserts a Service category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void InsertServiceCategoryMapping(ServiceCategoryMapping ServiceCategory)
        {
            if (ServiceCategory == null)
                throw new ArgumentNullException(nameof(ServiceCategory));

            _ServiceCategoryMappingRepository.Insert(ServiceCategory);

            //cache
            _cacheManager.RemoveByPattern(NopServiceDefaults.ServiceCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(ServiceCategory);
        }
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public IList<ServiceCategory> SortServiceCategoriesForTree(IList<ServiceCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<ServiceCategory>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortServiceCategoriesForTree(source, cat.Id, true));
            }

            if (ignoreCategoriesWithoutExistingParent || result.Count == source.Count)
                return result;

            //find categories without parent in provided category source and insert them into result
            foreach (var cat in source)
                if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                    result.Add(cat);

            return result;
        }
        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public void UpdateServiceCategory(ServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //validate category hierarchy
            var parentCategory = GetServiceCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }

                parentCategory = GetServiceCategoryById(parentCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //cache
            _cacheManager.RemoveByPattern(NopServiceDefaults.CategoriesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopServiceDefaults.CategoriesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopServiceDefaults.ServiceCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }
        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void UpdateServiceCategoryMapping(ServiceCategoryMapping ServiceCategory)
        {
            if (ServiceCategory == null)
                throw new ArgumentNullException(nameof(ServiceCategory));

            _ServiceCategoryMappingRepository.Update(ServiceCategory);

            //cache
            _cacheManager.RemoveByPattern(NopServiceDefaults.ServiceCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(ServiceCategory);
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public virtual string GetFormattedBreadCrumb(ServiceCategory category, IList<ServiceCategory> allCategories = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = this.GetServiceCategoryBreadCrumb(category, allCategories, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = _localizationService.GetLocalized(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? categoryName : $"{result} {separator} {categoryName}";
            }

            return result;
        }
        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public virtual IList<ServiceCategory> GetServiceCategoryBreadCrumb(ServiceCategory category, IList<ServiceCategory> allCategories = null, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var result = new List<ServiceCategory>();

            //used to prevent circular references
            var alreadyProcessedCategoryIds = new List<int>();

            while (category != null && //not null
                !category.Deleted && //not deleted
                (showHidden || category.Published) && //published
                (showHidden || _aclService.Authorize(category)) && //ACL
                (showHidden || _storeMappingService.Authorize(category)) && //Store mapping
                !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedCategoryIds.Add(category.Id);

                category = allCategories != null ? allCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId)
                    : this.GetServiceCategoryById(category.ParentCategoryId);
            }

            result.Reverse();
            return result;
        }

        public virtual ServiceCategory GetFirstByServiceId(int ServiceId)
        {
            var result = _ServiceCategoryMappingRepository.Table.Where(ncm => ncm.ServiceId == ServiceId).Select(ncm => ncm.Category).FirstOrDefault();
            return result;
        }
        #endregion

    }
}
