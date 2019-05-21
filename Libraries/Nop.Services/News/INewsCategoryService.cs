using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    public partial interface INewsCategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteNewsCategory(NewsCategory category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Categories</returns>
        IList<NewsCategory> GetAllNewsCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<NewsCategory> GetAllNewsCategories(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false);

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
        NewsCategory GetNewsCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertNewsCategory(NewsCategory category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateNewsCategory(NewsCategory category);

        /// <summary>
        /// Deletes a News category mapping
        /// </summary>
        /// <param name="NewsCategory">News category</param>
        void DeleteNewsCategoryMapping(NewsCategoryMapping newsCategory);

        /// <summary>
        /// Gets News category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News a category mapping collection</returns>
        IPagedList<NewsCategoryMapping> GetNewsCategoryMappingByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a News category mapping collection
        /// </summary>
        /// <param name="NewsId">News identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News category mapping collection</returns>
        IList<NewsCategoryMapping> GetNewsCategoryMappingByNewsId(int newsId, bool showHidden = false);

        /// <summary>
        /// Gets a News category mapping collection
        /// </summary>
        /// <param name="NewsId">News identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> News category mapping collection</returns>
        IList<NewsCategoryMapping> GetNewsCategoryMappingByNewsId(int NewsId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a News category mapping 
        /// </summary>
        /// <param name="NewsCategoryId">News category mapping identifier</param>
        /// <returns>News category mapping</returns>
        NewsCategoryMapping GetNewsCategoryMappingById(int NewsCategoryId);

        /// <summary>
        /// Inserts a News category mapping
        /// </summary>
        /// <param name="NewsCategory">>News category mapping</param>
        void InsertNewsCategoryMapping(NewsCategoryMapping NewsCategory);

        /// <summary>
        /// Updates the News category mapping 
        /// </summary>
        /// <param name="NewsCategory">>News category mapping</param>
        void UpdateNewsCategoryMapping(NewsCategoryMapping NewsCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        string[] GetNotExistingNewsCategories(string[] categoryIdsNames);

        /// <summary>
        /// Get category IDs for Newss
        /// </summary>
        /// <param name="NewsIds">Newss IDs</param>
        /// <returns>Category IDs for Newss</returns>
        IDictionary<int, int[]> GetNewsCategoryIds(int[] NewsIds);

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        IList<NewsCategory> SortNewsCategoriesForTree(IList<NewsCategory> source, int parentId = 0,
            bool ignoreCategoriesWithoutExistingParent = false);

        /// <summary>
        /// Returns a NewsCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="NewsId">News identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A NewsCategory that has the specified values; otherwise null</returns>
        NewsCategoryMapping FindNewsCategoryMapping(IList<NewsCategoryMapping> source, int NewsId, int categoryId);

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        string GetFormattedBreadCrumb(NewsCategory category, IList<NewsCategory> allCategories = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        IList<NewsCategory> GetNewsCategoryBreadCrumb(NewsCategory category, IList<NewsCategory> allCategories = null, bool showHidden = false);
        /// <summary>
        /// Get first category of News item
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        NewsCategory GetFirstByNewsId(int newsId);
    }
}
