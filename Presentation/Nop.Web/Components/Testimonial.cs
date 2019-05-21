using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class TestimonialViewComponent : NopViewComponent
    {
        private readonly ITestimonialModelFactory _testimonialModelFactory;
        private readonly CatalogSettings _catalogSettings;

        public TestimonialViewComponent(ITestimonialModelFactory testimonialModelFactory, CatalogSettings catalogSettings)
        {
            this._testimonialModelFactory = testimonialModelFactory;
            this._catalogSettings = catalogSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (!_catalogSettings.ShowTestimonialsOnHomepage)
            {
                return Content("");
            }
            var model = _testimonialModelFactory.PrepareHomeTestimonial();
            return View(model);
        }
    }
}
