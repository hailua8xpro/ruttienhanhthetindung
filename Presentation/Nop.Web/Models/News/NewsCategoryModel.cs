using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace Nop.Web.Models.News
{
    public class NewsCategoryModel : BaseNopEntityModel
    {
        public NewsCategoryModel()
        {
            PictureModel = new PictureModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<NewsCategoryModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<NewsCategoryModel> CategoryBreadcrumb { get; set; }

        public IList<SubCategoryModel> SubCategories { get; set; }

        public NewsItemListModel NewsItemListModel { get; set; }

        #region Nested Classes

        public partial class SubCategoryModel : BaseNopEntityModel
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public string Description { get; set; }

            public PictureModel PictureModel { get; set; }
        }

        #endregion
    }
}