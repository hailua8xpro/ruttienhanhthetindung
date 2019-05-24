using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Services;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class OtherServiceViewComponent : NopViewComponent
    {
        private readonly IServiceItemModelFactory _serviceModelFactory;

        public OtherServiceViewComponent(IServiceItemModelFactory ServiceModelFactory, ServiceSettings ServiceSettings)
        {
            this._serviceModelFactory = ServiceModelFactory;
        }

        public IViewComponentResult Invoke(int serviceId)
        {
            var model = _serviceModelFactory.PrepareOtherServiceItemsModel(serviceId);
            return View(model);
        }
    }
}
