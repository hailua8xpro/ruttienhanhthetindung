using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Testimonial;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Testimonials;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Testimonials;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class TestimonialController: BaseAdminController
    {
        #region Fields
        private readonly ITestimonialService _testimonialService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ITestimonialModelFactory _testimonialModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _cacheManager;


        #endregion

        #region Ctor

        public TestimonialController(ITestimonialService testimonialService,
            IAclService aclService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ITestimonialModelFactory testimonialModelFactory,
            IPermissionService permissionService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICategoryService categoryService,
            IWorkContext workContext,
            IStaticCacheManager cacheManager)
        {
            this._testimonialService = testimonialService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._testimonialModelFactory = testimonialModelFactory;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Testimonial testimonial, TestimonialModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(testimonial,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(testimonial,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(Testimonial testimonial)
        {
            var picture = _pictureService.GetPictureById(testimonial.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(testimonial.FullName));
        }
        protected virtual void SaveStoreMappings(Testimonial testimonial, TestimonialModel model)
        {
            testimonial.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(testimonial);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(testimonial, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTestimonials))
                return AccessDeniedView();

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialSearchModel(new TestimonialSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(TestimonialSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTestimonials))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialListModel(searchModel);

            return Json(model);
        }
        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTestimonials))
                return AccessDeniedView();

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialModel(new TestimonialModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(TestimonialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTestimonials))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var Testimonial = model.ToEntity<Testimonial>();
                Testimonial.CreatedOnUtc = DateTime.UtcNow;
                Testimonial.UpdatedOnUtc = DateTime.UtcNow;
                _testimonialService.InsertTestimonial(Testimonial);

                //locales
                UpdateLocales(Testimonial, model);

                //update picture seo file name
                UpdatePictureSeoNames(Testimonial);

                //stores
                SaveStoreMappings(Testimonial, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewTestimonial",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewTestimonial"), Testimonial.FullName), Testimonial);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Testimonials.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Testimonial.Id });
            }

            //prepare model
            model = _testimonialModelFactory.PrepareTestimonialModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTestimonials))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var Testimonial = _testimonialService.GetTestimonialById(id);
            if (Testimonial == null || Testimonial.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialModel(null, Testimonial);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(TestimonialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var Testimonial = _testimonialService.GetTestimonialById(model.Id);
            if (Testimonial == null || Testimonial.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = Testimonial.PictureId;
                Testimonial = model.ToEntity(Testimonial);
                Testimonial.UpdatedOnUtc = DateTime.UtcNow;
                _testimonialService.UpdateTestimonial(Testimonial);

                //locales
                UpdateLocales(Testimonial, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != Testimonial.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(Testimonial);

                //stores
                SaveStoreMappings(Testimonial, model);

                //activity log
                _customerActivityService.InsertActivity("EditTestimonial",
                    string.Format(_localizationService.GetResource("ActivityLog.EditTestimonial"), Testimonial.FullName), Testimonial);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Testimonials.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Testimonial.Id });
            }

            //prepare model
            model = _testimonialModelFactory.PrepareTestimonialModel(model, Testimonial, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var Testimonial = _testimonialService.GetTestimonialById(id);
            if (Testimonial == null)
                return RedirectToAction("List");

            _testimonialService.DeleteTestimonial(Testimonial);

            //activity log
            _customerActivityService.InsertActivity("DeleteTestimonial",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteTestimonial"), Testimonial.FullName), Testimonial);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}