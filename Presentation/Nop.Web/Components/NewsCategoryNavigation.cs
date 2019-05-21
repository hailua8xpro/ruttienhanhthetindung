using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class NewsCategoryNavigationViewComponent : NopViewComponent
    {
        private readonly INewsCategoryModelFactory _newsCategoryModelFactory;

        public NewsCategoryNavigationViewComponent(INewsCategoryModelFactory newsCategoryModelFactory)
        {
            this._newsCategoryModelFactory = newsCategoryModelFactory;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentNewsId)
        {
            var model = _newsCategoryModelFactory.PrepareNewsCategoryNavigationModel(currentCategoryId, currentNewsId);
            return View(model);
        }
    }
}
