using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Services
{
    public partial class ServiceCategoryNavigationModel : BaseNopModel
    {
        public ServiceCategoryNavigationModel()
        {
            ServiceCategories = new List<ServiceCategorySimpleModel>();
        }

        public int CurrentServiceCategoryId { get; set; }
        public List<ServiceCategorySimpleModel> ServiceCategories { get; set; }

        #region Nested classes

        public class ServiceCategoryLineModel : BaseNopModel
        {
            public int CurrentServiceCategoryId { get; set; }
            public ServiceCategorySimpleModel ServiceCategory { get; set; }
        }

        #endregion
    }
}