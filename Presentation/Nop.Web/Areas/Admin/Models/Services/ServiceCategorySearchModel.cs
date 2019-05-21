using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Services
{
    public class ServiceCategorySearchModel: BaseSearchModel
    {
        #region Ctor

        public ServiceCategorySearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        #endregion
    }
}
