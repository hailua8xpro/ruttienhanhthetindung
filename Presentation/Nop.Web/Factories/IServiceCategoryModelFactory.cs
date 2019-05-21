using Nop.Core.Domain.Services;
using Nop.Web.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
  public  interface IServiceCategoryModelFactory
    {
        ServiceCategoryModel PrepareServiceCategoryModel(ServiceCategory category, ServicePagingFilteringModel command);
        ServiceCategoryNavigationModel PrepareServiceCategoryNavigationModel(int currentCategoryId, int currentServiceId);
        List<ServiceCategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        List<ServiceCategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId, bool loadSubCategories = true);
    }
}
