using Nop.Core.Domain.News;
using Nop.Web.Models.News;
using System.Collections.Generic;

namespace Nop.Web.Factories
{
    public interface INewsCategoryModelFactory
    {
        NewsCategoryModel PrepareNewsCategoryModel(NewsCategory category, NewsPagingFilteringModel command);
        NewsCategoryNavigationModel PrepareNewsCategoryNavigationModel(int currentCategoryId, int currentNewsId);
        List<NewsCategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        List<NewsCategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId, bool loadSubCategories = true);
    }
}
