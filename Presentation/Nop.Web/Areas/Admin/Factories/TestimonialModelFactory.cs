using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Domain.Testimonial;
using Nop.Web.Areas.Admin.Models.Testimonials;
using Nop.Services.Localization;
using Nop.Web.Framework.Factories;
using Nop.Services.Testimonials;
using Nop.Services.Media;

namespace Nop.Web.Areas.Admin.Factories
{
    public class TestimonialModelFactory : ITestimonialModelFactory
    {
        #region fields
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ITestimonialService _testimonialService;
        private readonly IPictureService _pictureService;
        #endregion
        #region ctor
        public TestimonialModelFactory(ILocalizationService localizationService,
            IPictureService pictureService,
            ILocalizedModelFactory localizedModelFactory,
            ITestimonialService testimonialService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._testimonialService = testimonialService;
            this._pictureService = pictureService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }
        #endregion
        #region method
        public TestimonialSearchModel PrepareTestimonialSearchModel(TestimonialSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);
            //prepare page parameters
            searchModel.SetGridPageSize();
            return searchModel;
        }
        public TestimonialListModel PrepareTestimonialListModel(TestimonialSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var Testimonials = _testimonialService.GetAllTestimonials(
                keyword:searchModel.SearchFullName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new TestimonialListModel
            {
                //fill in model values from the entity
                Data = Testimonials.Select(Testimonial => {
                    var Testimonialmodel = Testimonial.ToModel<TestimonialModel>();
                    Testimonialmodel.ImageUrl = _pictureService.GetPictureUrl(Testimonialmodel.PictureId, 120);
                    return Testimonialmodel;
                }),
                Total = Testimonials.TotalCount
            };

            return model;
        }

        public TestimonialModel PrepareTestimonialModel(TestimonialModel model, Testimonial testimonial, bool excludeProperties = false)
        {
            Action<TestimonialLocalizedModel, int> localizedModelConfiguration = null;

            if (testimonial != null)
            {
                //fill in model values from the entity
                model = model ?? testimonial.ToModel<TestimonialModel>();
                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Description = _localizationService.GetLocalized(testimonial, entity => entity.Description, languageId, false, false);
                    locale.FullDescription = _localizationService.GetLocalized(testimonial, entity => entity.FullDescription, languageId, false, false);
                };
            }

            //set default values for the new model
            if (testimonial == null)
            {
                model.Published = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
            _storeMappingSupportedModelFactory.PrepareModelStores(model, testimonial, excludeProperties);
            _baseAdminModelFactory.PrepareStores(model.AvailableStores);
            return model;
        }
        #endregion

    }
}
