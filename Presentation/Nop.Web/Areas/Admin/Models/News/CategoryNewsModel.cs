using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News
{
    public class NewsCategoryMappingModel: BaseNopEntityModel
    {
        #region Properties

        public int CategoryId { get; set; }

        public int NewsId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.News.NewsCategories.Fields.News")]
        public string Title { get; set; }
        #endregion
    }
}
