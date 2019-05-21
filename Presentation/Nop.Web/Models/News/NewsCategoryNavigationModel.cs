using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.News;

namespace Nop.Web.Models.News
{
    public partial class NewsCategoryNavigationModel : BaseNopModel
    {
        public NewsCategoryNavigationModel()
        {
            NewsCategories = new List<NewsCategorySimpleModel>();
        }

        public int CurrentNewsCategoryId { get; set; }
        public List<NewsCategorySimpleModel> NewsCategories { get; set; }

        #region Nested classes

        public class NewsCategoryLineModel : BaseNopModel
        {
            public int CurrentNewsCategoryId { get; set; }
            public NewsCategorySimpleModel NewsCategory { get; set; }
        }

        #endregion
    }
}