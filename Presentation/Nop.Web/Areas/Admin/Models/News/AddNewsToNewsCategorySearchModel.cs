using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.News
{
    public class AddNewsToNewsCategorySearchModel:BaseSearchModel
    {
        #region ctor
        public AddNewsToNewsCategorySearchModel()
        {
            AvailableNewsCategories = new List<SelectListItem>();
        }
        #endregion
        #region properties
        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchNewsTitle")]
        public string SearchNewsTitle { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchNewsCategory")]
        public int SearchNewsCategoryId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableNewsCategories { get; set; }

        #endregion
    }
}
