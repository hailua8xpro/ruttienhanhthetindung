using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomeSmallBannerViewComponent : NopViewComponent
    {
        private readonly IBannerModelFactory _bannerModelFactory;
        private readonly IStoreContext _storeContext;

        public HomeSmallBannerViewComponent(IBannerModelFactory bannerModelFactory, IStoreContext storeContext)
        {
            this._bannerModelFactory = bannerModelFactory;
            this._storeContext = storeContext;
        }

        public IViewComponentResult Invoke(int type, int categoryId)
        {
            var model = _bannerModelFactory.PrepareBannerModel(_storeContext.CurrentStore.Id, type, categoryId);
            return View(model);
        }
    }
}