using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Banners;
using Nop.Services.Banners;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class BannerController : BaseAdminController
    {
        #region Fields
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IBannerModelFactory _bannerModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IBannerService _bannerService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IStaticCacheManager _cacheManager;


        #endregion

        #region Ctor

        public BannerController(IAclService aclService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IBannerModelFactory bannerModelFactory,
            IPermissionService permissionService,
            IBannerService bannerService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICategoryService categoryService,
            IStaticCacheManager cacheManager)
        {
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._bannerModelFactory = bannerModelFactory;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._bannerService = bannerService;
            this._categoryService = categoryService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Banner banner, BannerModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(banner,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(banner,
                    x => x.Caption,
                    localized.Caption,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(Banner banner)
        {
            var picture = _pictureService.GetPictureById(banner.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(banner.Title));
        }
        protected virtual void SaveStoreMappings(Banner banner, BannerModel model)
        {
            banner.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(banner);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(banner, store.Id);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerSearchModel(new BannerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(BannerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerListModel(searchModel);

            return Json(model);
        }
        public virtual IActionResult GetCategoriesByBannerType(int type = 0)
        {
            BannerType bannertype = (BannerType)type;
            switch (bannertype)
            {
                case BannerType.Home:
                    return Json(null);
                case BannerType.ProductCategory:
                    //var categories = _categoryService.GetAllCategories();
                    //var result = _categoryService.GetAllCategories().Select(x => new { id = x.Id, name = x.Name }).ToList();
                    //result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Common.All") });
                    var result = SelectListHelper.GetCategoryList(_categoryService, _cacheManager);
                    return Json(result);
                default:
                    break;
            }
            return Json(null);
        }
        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerModel(new BannerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var banner = model.ToEntity<Banner>();
                banner.CreatedOnUtc = DateTime.UtcNow;
                banner.UpdatedOnUtc = DateTime.UtcNow;
                _bannerService.InsertBanner(banner);

                //locales
                UpdateLocales(banner, model);

                //update picture seo file name
                UpdatePictureSeoNames(banner);

                //stores
                SaveStoreMappings(banner, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewBanner",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewBanner"), banner.Title), banner);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Banners.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = banner.Id });
            }

            //prepare model
            model = _bannerModelFactory.PrepareBannerModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var banner = _bannerService.GetBannerById(id);
            if (banner == null || banner.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _bannerModelFactory.PrepareBannerModel(null, banner);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var banner = _bannerService.GetBannerById(model.Id);
            if (banner == null || banner.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = banner.PictureId;
                banner = model.ToEntity(banner);
                banner.UpdatedOnUtc = DateTime.UtcNow;
                _bannerService.UpdateBanner(banner);

                //locales
                UpdateLocales(banner, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != banner.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(banner);

                //stores
                SaveStoreMappings(banner, model);

                //activity log
                _customerActivityService.InsertActivity("EditBanner",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBanner"), banner.Title), banner);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Banners.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = banner.Id });
            }

            //prepare model
            model = _bannerModelFactory.PrepareBannerModel(model, banner, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var banner = _bannerService.GetBannerById(id);
            if (banner == null)
                return RedirectToAction("List");

            _bannerService.DeleteBanner(banner);

            //activity log
            _customerActivityService.InsertActivity("DeleteBanner",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteBanner"), banner.Title), banner);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}