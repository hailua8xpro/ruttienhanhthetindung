using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Services
{
    public class ServiceCategorySimpleModel : BaseNopEntityModel
    {
        public ServiceCategorySimpleModel()
        {
            SubCategories = new List<ServiceCategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<ServiceCategorySimpleModel> SubCategories { get; set; }
    }
}
