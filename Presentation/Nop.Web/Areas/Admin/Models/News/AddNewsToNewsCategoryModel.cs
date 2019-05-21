using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.News
{
    public class AddNewsToNewsCategoryModel : BaseNopModel
    {
        #region Ctor

        public AddNewsToNewsCategoryModel()
        {
            SelectedNewsIds = new List<int>();
        }
        #endregion

        #region Properties

        public int NewsCategoryId { get; set; }

        public IList<int> SelectedNewsIds { get; set; }

        #endregion
    }
}
