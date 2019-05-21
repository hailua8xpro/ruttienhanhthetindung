using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.News
{
    public class NewsCategorySimpleModel : BaseNopEntityModel
    {
        public NewsCategorySimpleModel()
        {
            SubCategories = new List<NewsCategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<NewsCategorySimpleModel> SubCategories { get; set; }
    }
}
