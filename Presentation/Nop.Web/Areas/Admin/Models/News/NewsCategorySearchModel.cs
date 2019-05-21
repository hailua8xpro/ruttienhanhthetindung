using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.News
{
    public class NewsCategorySearchModel: BaseSearchModel
    {
        #region Ctor

        public NewsCategorySearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.News.Categories.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.News.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        #endregion
    }
}
