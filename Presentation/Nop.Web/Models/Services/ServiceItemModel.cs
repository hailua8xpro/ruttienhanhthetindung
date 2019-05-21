using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Services
{
    public class ServiceItemModel: BaseNopEntityModel
    {
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public PictureModel PictureModel { get; set; }
        public string Name { get; set; }
        public string Short { get; set; }
        public string Full { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ServiceCategoryName { get; set; }
        public string ServiceCategorySeName { get; set; }
        public IList<ServiceCategorySimpleModel> CategoryBreadcrumb { get; set; }

    }
}
