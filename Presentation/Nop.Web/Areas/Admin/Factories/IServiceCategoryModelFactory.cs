using Nop.Core.Domain.Services;
using Nop.Web.Areas.Admin.Models.Services;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface IServiceCategoryModelFactory
    {
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        ServiceCategorySearchModel PrepareServiceCategorySearchModel(ServiceCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category list model</returns>
        ServiceCategoryListModel PrepareCategoryListModel(ServiceCategorySearchModel searchModel);

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        ServiceCategoryModel PrepareServiceCategoryModel(ServiceCategoryModel model, ServiceCategory category, bool excludeProperties = false);
        /// <summary>
        /// Prepare paged category Service list model
        /// </summary>
        /// <param name="searchModel">Category Service search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category Service list model</returns>
        ServiceCategoryMappingListModel PrepareCategoryServiceListModel(CategoryServiceSearchModel searchModel, ServiceCategory category);
        /// <summary>
        /// Prepare Service search  model
        /// </summary>
        /// <param name="addServiceToServiceCategorySearchModel"></param>
        /// <returns></returns>
        AddServiceToServiceCategorySearchModel PrepareAddServiceToServiceCategorySearchModel(AddServiceToServiceCategorySearchModel addServiceToServiceCategorySearchModel);
        /// <summary>
        /// Prepare paged Service list model to add to the Service category
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        AddServiceToServiceCategoryListModel PrepareAddServiceToServiceCategoryListModel(AddServiceToServiceCategorySearchModel searchModel);
    }
}
