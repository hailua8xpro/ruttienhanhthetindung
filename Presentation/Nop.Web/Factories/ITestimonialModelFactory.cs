using Nop.Web.Models.Testimonials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public interface ITestimonialModelFactory
    {
        IList<TestimonialModel> PrepareHomeTestimonial();
    }
}
