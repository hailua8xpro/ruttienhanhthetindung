using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Services;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageServiceViewComponent : NopViewComponent
    {
        private readonly IServiceItemModelFactory _serviceModelFactory;
        private readonly ServiceSettings _serviceSettings;

        public HomepageServiceViewComponent(IServiceItemModelFactory ServiceModelFactory, ServiceSettings ServiceSettings)
        {
            this._serviceModelFactory = ServiceModelFactory;
            this._serviceSettings = ServiceSettings;
        }

        public IViewComponentResult Invoke()
        {
            var model = _serviceModelFactory.PrepareHomePageServiceItemsModel();
            return View(model);
        }
    }
}
