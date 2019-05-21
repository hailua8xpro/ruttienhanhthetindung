using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Services
{
    public class AddServiceToServiceCategorySearchModel:BaseSearchModel
    {
        #region ctor
        public AddServiceToServiceCategorySearchModel()
        {
            AvailableServiceCategories = new List<SelectListItem>();
        }
        #endregion
        #region properties
        [NopResourceDisplayName("Admin.Catalog.Service.ServiceItems.List.SearchServiceName")]
        public string SearchServiceName{ get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.ServiceItems.List.SearchServiceCategory")]
        public int SearchServiceCategoryId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Service.ServiceItems.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableServiceCategories { get; set; }

        #endregion
    }
}
