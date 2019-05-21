using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ServiceCategoryNavigationViewComponent : NopViewComponent
    {
        private readonly IServiceCategoryModelFactory _serviceCategoryModelFactory;

        public ServiceCategoryNavigationViewComponent(IServiceCategoryModelFactory serviceCategoryModelFactory)
        {
            this._serviceCategoryModelFactory = serviceCategoryModelFactory;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentServiceId)
        {
            var model = _serviceCategoryModelFactory.PrepareServiceCategoryNavigationModel(currentCategoryId, currentServiceId);
            return View(model);
        }
    }
}
