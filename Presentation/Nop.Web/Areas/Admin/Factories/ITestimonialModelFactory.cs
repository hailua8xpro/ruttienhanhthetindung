using Nop.Core.Domain.Testimonial;
using Nop.Web.Areas.Admin.Models.Testimonials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface ITestimonialModelFactory
    {
        TestimonialSearchModel PrepareTestimonialSearchModel(TestimonialSearchModel searchModel);
        TestimonialListModel PrepareTestimonialListModel(TestimonialSearchModel searchModel);
        TestimonialModel PrepareTestimonialModel(TestimonialModel model,
            Testimonial testimonial, bool excludeProperties = false);
    }
}
