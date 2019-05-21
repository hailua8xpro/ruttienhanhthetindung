using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class MobileMenuViewComponent : NopViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public MobileMenuViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            this._catalogModelFactory = catalogModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _catalogModelFactory.PrepareTopMenuModel();
            return View(model);
        }
    }
}
