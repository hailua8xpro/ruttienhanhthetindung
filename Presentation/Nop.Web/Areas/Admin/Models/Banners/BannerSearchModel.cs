using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Banners;

namespace Nop.Web.Areas.Admin.Models.Banners
{
    public class BannerSearchModel : BaseSearchModel
    {
        #region Ctor

        public BannerSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableCategoryEntity = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        [NopResourceDisplayName("Admin.ContentManagement.Banners.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Type")]
        public int Type { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.CategoryId")]
        public int CategoryId { get; set; }
        public IList<SelectListItem> AvailableCategoryEntity { get; set; }

        #endregion
    }
}
