using Nop.Core;
using Nop.Core.Domain.Testimonial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Testimonials
{
    public interface ITestimonialService
    {
        void DeleteTestimonial(Testimonial testimonial);
        Testimonial GetTestimonialById(int testimonialId);
        IPagedList<Testimonial> GetAllTestimonials(string keyword,int storeId = 0,int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        void InsertTestimonial(Testimonial Testimonial);

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void UpdateTestimonial(Testimonial Testimonial);
        IList<Testimonial> PrepareHomeTestimonial(int storeId);
    }
}
