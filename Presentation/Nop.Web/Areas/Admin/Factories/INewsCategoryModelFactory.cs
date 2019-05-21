using Nop.Core.Domain.News;
using Nop.Web.Areas.Admin.Models.News;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface INewsCategoryModelFactory
    {
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        NewsCategorySearchModel PrepareNewsCategorySearchModel(NewsCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category list model</returns>
        NewsCategoryListModel PrepareCategoryListModel(NewsCategorySearchModel searchModel);

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        NewsCategoryModel PrepareNewsCategoryModel(NewsCategoryModel model, NewsCategory category, bool excludeProperties = false);
        /// <summary>
        /// Prepare paged category news list model
        /// </summary>
        /// <param name="searchModel">Category news search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category news list model</returns>
        NewsCategoryMappingListModel PrepareCategoryNewsListModel(CategoryNewsSearchModel searchModel, NewsCategory category);
        /// <summary>
        /// Prepare news search  model
        /// </summary>
        /// <param name="addNewsToNewsCategorySearchModel"></param>
        /// <returns></returns>
        AddNewsToNewsCategorySearchModel PrepareAddNewsToNewsCategorySearchModel(AddNewsToNewsCategorySearchModel addNewsToNewsCategorySearchModel);
        /// <summary>
        /// Prepare paged news list model to add to the news category
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        AddNewsToNewsCategoryListModel PrepareAddNewsToNewsCategoryListModel(AddNewsToNewsCategorySearchModel searchModel);
    }
}
