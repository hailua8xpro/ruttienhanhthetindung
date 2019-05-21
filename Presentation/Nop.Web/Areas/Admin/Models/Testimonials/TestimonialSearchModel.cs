using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Testimonials
{
    public class TestimonialSearchModel : BaseSearchModel
    {
        #region Ctor

        public TestimonialSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        [NopResourceDisplayName("Admin.ContentManagement.Testimonial.List.SearchFullName")]
        public string SearchFullName { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonial.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        #endregion
    }
}
