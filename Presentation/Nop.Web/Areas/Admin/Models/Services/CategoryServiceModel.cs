using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Services
{
    public class ServiceCategoryMappingModel: BaseNopEntityModel
    {
        #region Properties

        public int CategoryId { get; set; }

        public int ServiceId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Service.ServiceCategories.Fields.Service")]
        public string Name { get; set; }
        #endregion
    }
}
