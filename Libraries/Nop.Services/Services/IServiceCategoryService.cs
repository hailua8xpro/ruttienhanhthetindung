using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Services;

namespace Nop.Services.Services
{
    public partial interface IServiceCategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteServiceCategory(ServiceCategory category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Categories</returns>
        IList<ServiceCategory> GetAllServiceCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<ServiceCategory> GetAllServiceCategories(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<ServiceCategory> GetAllServiceCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false);

        /// <summary>
        /// Gets child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category identifiers</returns>
        IList<int> GetChildCategoryIds(int parentCategoryId, int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        ServiceCategory GetServiceCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertServiceCategory(ServiceCategory category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateServiceCategory(ServiceCategory category);

        /// <summary>
        /// Deletes a Service category mapping
        /// </summary>
        /// <param name="ServiceCategory">Service category</param>
        void DeleteServiceCategoryMapping(ServiceCategoryMapping ServiceCategory);

        /// <summary>
        /// Gets Service category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Service a category mapping collection</returns>
        IPagedList<ServiceCategoryMapping> GetServiceCategoryMappingByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a Service category mapping collection
        /// </summary>
        /// <param name="ServiceId">Service identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Service category mapping collection</returns>
        IList<ServiceCategoryMapping> GetServiceCategoryMappingByServiceId(int ServiceId, bool showHidden = false);

        /// <summary>
        /// Gets a Service category mapping collection
        /// </summary>
        /// <param name="ServiceId">Service identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Service category mapping collection</returns>
        IList<ServiceCategoryMapping> GetServiceCategoryMappingByServiceId(int ServiceId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a Service category mapping 
        /// </summary>
        /// <param name="ServiceCategoryId">Service category mapping identifier</param>
        /// <returns>Service category mapping</returns>
        ServiceCategoryMapping GetServiceCategoryMappingById(int ServiceCategoryId);

        /// <summary>
        /// Inserts a Service category mapping
        /// </summary>
        /// <param name="ServiceCategory">>Service category mapping</param>
        void InsertServiceCategoryMapping(ServiceCategoryMapping ServiceCategory);

        /// <summary>
        /// Updates the Service category mapping 
        /// </summary>
        /// <param name="ServiceCategory">>Service category mapping</param>
        void UpdateServiceCategoryMapping(ServiceCategoryMapping ServiceCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        string[] GetNotExistingServiceCategories(string[] categoryIdsNames);

        /// <summary>
        /// Get category IDs for Services
        /// </summary>
        /// <param name="ServiceIds">Services IDs</param>
        /// <returns>Category IDs for Services</returns>
        IDictionary<int, int[]> GetServiceCategoryIds(int[] ServiceIds);

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        IList<ServiceCategory> SortServiceCategoriesForTree(IList<ServiceCategory> source, int parentId = 0,
            bool ignoreCategoriesWithoutExistingParent = false);

        /// <summary>
        /// Returns a ServiceCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="ServiceId">Service identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A ServiceCategory that has the specified values; otherwise null</returns>
        ServiceCategoryMapping FindServiceCategoryMapping(IList<ServiceCategoryMapping> source, int ServiceId, int categoryId);

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        string GetFormattedBreadCrumb(ServiceCategory category, IList<ServiceCategory> allCategories = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        IList<ServiceCategory> GetServiceCategoryBreadCrumb(ServiceCategory category, IList<ServiceCategory> allCategories = null, bool showHidden = false);
        /// <summary>
        /// Get first category of Service item
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <returns></returns>
        ServiceCategory GetFirstByServiceId(int ServiceId);
    }
}
