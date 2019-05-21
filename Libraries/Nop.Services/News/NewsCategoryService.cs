using System;
using System.Linq;

using System.Collections.Generic;
using Nop.Core.Caching;
using Nop.Services.Events;
using Nop.Core;
using Nop.Core.Data;

using Nop.Core.Domain.News;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Localization;

namespace Nop.Services.News
{
    public partial class NewsCategoryService : INewsCategoryService
    {
        #region Fields
        private readonly Core.Domain.Catalog.CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<NewsCategory> _categoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly string _entityName;
        private readonly IRepository<NewsCategoryMapping> _newsCategoryMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<NewsItem> _newsRepository;
        private readonly ILocalizationService _localizationService;

        #endregion
        #region Ctor
        public NewsCategoryService(
            Core.Domain.Catalog.CatalogSettings catalogSettings,
            IAclService aclService,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            IStaticCacheManager staticCacheManager,
            IRepository<NewsCategory> categoryRepository,
            IRepository<AclRecord>  aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IRepository<NewsCategoryMapping> newsCategoryRepository,
            IRepository<NewsItem> newsRepository,
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
            this._entityName = typeof(NewsCategory).Name;
            this._newsCategoryMappingRepository = newsCategoryRepository;
            this._storeMappingService = storeMappingService;
            this._newsRepository = newsRepository;
            this._localizationService = localizationService;
        }
        #endregion
        #region Method
        public void DeleteNewsCategory(NewsCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            category.Deleted = true;
            UpdateNewsCategory(category);

            //event notification
            _eventPublisher.EntityDeleted(category);

            //reset a "Parent category" property of all child subcategories
            var subcategories = GetAllNewsCategoriesByParentCategoryId(category.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateNewsCategory(subcategory);
            }
        }

        public void DeleteNewsCategoryMapping(NewsCategoryMapping newsCategory)
        {
            throw new NotImplementedException();
        }

        public NewsCategoryMapping FindNewsCategoryMapping(IList<NewsCategoryMapping> source, int newsId, int categoryId)
        {
            foreach (var newsCategoryMapping in source)
                if (newsCategoryMapping.NewsId == newsId && newsCategoryMapping.CategoryId == categoryId)
                    return newsCategoryMapping;

            return null;
        }

        public IList<NewsCategory> GetAllNewsCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true)
        {
            IList<NewsCategory> LoadCategoriesFunc() => GetAllNewsCategories(string.Empty, storeId, showHidden: showHidden);

            IList<NewsCategory> categories;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(NopNewsDefaults.CategoriesAllCacheKey,
                    storeId,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    showHidden);
                categories = _staticCacheManager.Get(key, () =>
                {
                    var result = new List<NewsCategory>();
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

        public IPagedList<NewsCategory> GetAllNewsCategories(string categoryName, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
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
            var sortedCategories = this.SortNewsCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<NewsCategory>(sortedCategories, pageIndex, pageSize);
        }

        public IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false)
        {
            var key = string.Format(NopNewsDefaults.CategoriesByParentCategoryIdCacheKey, parentCategoryId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
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

        public NewsCategory GetNewsCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            var key = string.Format(NopNewsDefaults.CategoriesByIdCacheKey, categoryId);
            return _cacheManager.Get(key, () => _categoryRepository.GetById(categoryId));
        }

        public IList<int> GetChildCategoryIds(int parentCategoryId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = string.Format(NopNewsDefaults.CategoriesChildIdentifiersCacheKey,
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
                var categories = GetAllNewsCategories(storeId: storeId, showHidden: showHidden)
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
        public IPagedList<NewsCategoryMapping> GetNewsCategoryMappingByCategoryId(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<NewsCategoryMapping>(new List<NewsCategoryMapping>(), pageIndex, pageSize);

            var key = string.Format(NopNewsDefaults.NewsCategoriesAllByCategoryIdCacheKey, showHidden, categoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from nc in _newsCategoryMappingRepository.Table
                            join p in _newsRepository.Table on nc.NewsId equals p.Id
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

                var newsCategories = new PagedList<NewsCategoryMapping>(query, pageIndex, pageSize);
                return newsCategories;
            });
        }
        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="newsId">Product identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public IList<NewsCategoryMapping> GetNewsCategoryMappingByNewsId(int newsId, bool showHidden = false)
        {
            return GetNewsCategoryMappingByNewsId(newsId, _storeContext.CurrentStore.Id, showHidden);
        }
        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="NewsId">News identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> News category mapping collection</returns>
        public IList<NewsCategoryMapping> GetNewsCategoryMappingByNewsId(int NewsId, int storeId, bool showHidden = false)
        {
            if (NewsId == 0)
                return new List<NewsCategoryMapping>();

            var key = string.Format(NopNewsDefaults.NewsCategoriesAllByNewsIdCacheKey, showHidden, NewsId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _newsCategoryMappingRepository.Table
                            join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                            where pc.NewsId == NewsId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.Id descending
                            select pc;

                var allNewsCategories = query.ToList();
                var result = new List<NewsCategoryMapping>();
                if (!showHidden)
                {
                    foreach (var pc in allNewsCategories)
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
                    result.AddRange(allNewsCategories);
                }

                return result;
            });
        }
        /// <summary>
        /// Gets a news category mapping 
        /// </summary>
        /// <param name="newsCategoryId">News category mapping identifier</param>
        /// <returns>News category mapping</returns>
        public NewsCategoryMapping GetNewsCategoryMappingById(int newsCategoryId)
        {
            if (newsCategoryId == 0)
                return null;

            return _newsCategoryMappingRepository.GetById(newsCategoryId);
        }
        /// <summary>
        /// Get category IDs for news
        /// </summary>
        /// <param name="newsIds">News IDs</param>
        /// <returns>Category IDs for news</returns>
        public IDictionary<int, int[]> GetNewsCategoryIds(int[] newsIds)
        {
            var query = _newsCategoryMappingRepository.Table;

            return query.Where(p => newsIds.Contains(p.NewsId))
                .Select(p => new { p.NewsId, p.CategoryId }).ToList()
                .GroupBy(a => a.NewsId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.CategoryId).ToArray());
        }
        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        public string[] GetNotExistingNewsCategories(string[] categoryIdsNames)
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
        public void InsertNewsCategory(NewsCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _categoryRepository.Insert(category);

            //cache
            _cacheManager.RemoveByPattern(NopNewsDefaults.CategoriesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopNewsDefaults.CategoriesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopNewsDefaults.NewsCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(category);
        }
        /// <summary>
        /// Inserts a news category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void InsertNewsCategoryMapping(NewsCategoryMapping newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            _newsCategoryMappingRepository.Insert(newsCategory);

            //cache
            _cacheManager.RemoveByPattern(NopNewsDefaults.NewsCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(newsCategory);
        }
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public IList<NewsCategory> SortNewsCategoriesForTree(IList<NewsCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<NewsCategory>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortNewsCategoriesForTree(source, cat.Id, true));
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
        public void UpdateNewsCategory(NewsCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //validate category hierarchy
            var parentCategory = GetNewsCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }

                parentCategory = GetNewsCategoryById(parentCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //cache
            _cacheManager.RemoveByPattern(NopNewsDefaults.CategoriesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopNewsDefaults.CategoriesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopNewsDefaults.NewsCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }
        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void UpdateNewsCategoryMapping(NewsCategoryMapping newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            _newsCategoryMappingRepository.Update(newsCategory);

            //cache
            _cacheManager.RemoveByPattern(NopNewsDefaults.NewsCategoriesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(newsCategory);
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
        public virtual string GetFormattedBreadCrumb(NewsCategory category, IList<NewsCategory> allCategories = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = this.GetNewsCategoryBreadCrumb(category, allCategories, true);
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
        public virtual IList<NewsCategory> GetNewsCategoryBreadCrumb(NewsCategory category, IList<NewsCategory> allCategories = null, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var result = new List<NewsCategory>();

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
                    : this.GetNewsCategoryById(category.ParentCategoryId);
            }

            result.Reverse();
            return result;
        }

        public virtual NewsCategory GetFirstByNewsId(int newsId)
        {
            var result = _newsCategoryMappingRepository.Table.Where(ncm => ncm.NewsId == newsId).Select(ncm => ncm.Category).FirstOrDefault();
            return result;
        }
        #endregion

    }
}
