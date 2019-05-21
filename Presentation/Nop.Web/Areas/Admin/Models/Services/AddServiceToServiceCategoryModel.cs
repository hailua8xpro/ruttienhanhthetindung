using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Services
{
    public class AddServiceToServiceCategoryModel : BaseNopModel
    {
        #region Ctor

        public AddServiceToServiceCategoryModel()
        {
            SelectedServiceIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ServiceCategoryId { get; set; }

        public IList<int> SelectedServiceIds { get; set; }

        #endregion
    }
}
