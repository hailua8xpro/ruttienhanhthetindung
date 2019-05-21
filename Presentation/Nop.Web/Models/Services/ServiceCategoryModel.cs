using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Services
{
    public class ServiceCategoryModel: BaseNopEntityModel
    {
        public ServiceCategoryModel()
        {
            PictureModel = new PictureModel();
            SubCategories = new List<SubCategoryModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool DisplayCategoryBreadcrumb { get; set; }
        public PictureModel PictureModel { get; set; }
        public IList<SubCategoryModel> SubCategories { get; set; }
        public ServiceItemListModel ServiceItemListModel { get; set; }
        public IList<ServiceCategoryModel> CategoryBreadcrumb { get; set; }
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
