using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Media;
using Nop.Services.Testimonials;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Testimonials;

namespace Nop.Web.Factories
{
    public class TestimonialModelFactory : ITestimonialModelFactory
    {
        #region field
        private readonly ITestimonialService _testimonialService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        #endregion
        #region ctor
        public TestimonialModelFactory(ITestimonialService testimonialService,
            IPictureService pictureService,
            IStoreContext storeContext,
            IStaticCacheManager cacheManager)
        {
            this._testimonialService = testimonialService;
            this._pictureService =pictureService;
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
        }
        #endregion
        #region Method
        public IList<TestimonialModel> PrepareHomeTestimonial()
        {
            var cacheKey= string.Format(ModelCacheEventConsumer.HOMEPAGE_TESTIMOMIAL_KEY, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => {
                var testimonials = _testimonialService.PrepareHomeTestimonial(_storeContext.CurrentStore.Id);
                return testimonials.Select(t => {
                    var picture = _pictureService.GetPictureById(t.PictureId);
                    var testimonialModel = new TestimonialModel {
                        ImageUrl=_pictureService.GetPictureUrl(picture,targetSize:90),
                        Description=t.Description,
                        FullDescription=t.FullDescription,
                        FullName=t.FullName
                    };
                    return testimonialModel;
                }).ToList();
            });
        }
        #endregion

    }
}
