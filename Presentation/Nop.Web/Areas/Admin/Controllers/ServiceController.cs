using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Services;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Services;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ServiceController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IServiceModelFactory _serviceModelFactory;
        private readonly IServiceService _serviceService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IPictureService _pictureService;


        #endregion

        #region Ctor

        public ServiceController(ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IServiceModelFactory ServiceModelFactory,
            IServiceService ServiceService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IServiceCategoryService ServiceCategoryService,
            IPictureService pictureService)
        {
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._serviceModelFactory = ServiceModelFactory;
            this._serviceService = ServiceService;
            this._permissionService = permissionService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._serviceCategoryService = ServiceCategoryService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities
        protected virtual void UpdatePictureSeoNames(Service Service)
        {
            var picture = _pictureService.GetPictureById(Service.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(Service.Name));
        }

        protected virtual void SaveStoreMappings(Service Service, ServiceModel model)
        {
            Service.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(Service);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(Service, store.Id);
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
        protected virtual void SaveCategoryMappings(Service Service, ServiceModel model)
        {
            var existingServiceCategoryMappings = _serviceCategoryService.GetServiceCategoryMappingByServiceId(Service.Id, true);

            //delete categories
            foreach (var existingServiceCategoryMapping in existingServiceCategoryMappings)
                if (!model.SelectedServiceCategoryIds.Contains(existingServiceCategoryMapping.CategoryId))
                    _serviceCategoryService.DeleteServiceCategoryMapping(existingServiceCategoryMapping);

            //add categories
            foreach (var categoryId in model.SelectedServiceCategoryIds)
            {
                if (_serviceCategoryService.FindServiceCategoryMapping(existingServiceCategoryMappings, Service.Id, categoryId) == null)
                {
                    _serviceCategoryService.InsertServiceCategoryMapping(new ServiceCategoryMapping
                    {
                        ServiceId = Service.Id,
                        CategoryId = categoryId,
                    });
                }
            }
        }
        #endregion

        #region Methods        

        #region Service items

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            //prepare model
            var model = _serviceModelFactory.PrepareServiceSearchModel(new ServiceSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ServiceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _serviceModelFactory.PrepareServiceListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ServiceCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            //prepare model
            var model = _serviceModelFactory.PrepareServiceModel(new ServiceModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ServiceCreate(ServiceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var service = model.ToEntity<Service>();
                service.CreatedOnUtc = DateTime.UtcNow;
                _serviceService.InsertService(service);

                //activity log
                _customerActivityService.InsertActivity("AddNewService",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewService"), service.Id), service);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(service, model.SeName, model.Name, true);
                _urlRecordService.SaveSlug(service, seName,0);
                //update picture seo file name
                UpdatePictureSeoNames(service);
                //Stores
                SaveStoreMappings(service, model);
                SaveCategoryMappings(service, model);
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Services.Item.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("ServiceEdit", new { id = service.Id });
            }

            //prepare model
            model = _serviceModelFactory.PrepareServiceModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ServiceEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            //try to get a Service item with the specified id
            var Service = _serviceService.GetServiceById(id);
            if (Service == null)
                return RedirectToAction("List");

            //prepare model
            var model = _serviceModelFactory.PrepareServiceModel(null, Service);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ServiceEdit(ServiceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            //try to get a Service item with the specified id
            var Service = _serviceService.GetServiceById(model.Id);
            if (Service == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = Service.PictureId;
                Service = model.ToEntity(Service);
                _serviceService.UpdateService(Service);

                //activity log
                _customerActivityService.InsertActivity("EditService",
                    string.Format(_localizationService.GetResource("ActivityLog.EditService"), Service.Id), Service);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(Service, model.SeName, model.Name, true);
                _urlRecordService.SaveSlug(Service, seName, 0);
                if (prevPictureId > 0 && prevPictureId != Service.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(Service);
                //stores
                SaveStoreMappings(Service, model);
                SaveCategoryMappings(Service, model);
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Services.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("ServiceEdit", new { id = Service.Id });
            }

            //prepare model
            model = _serviceModelFactory.PrepareServiceModel(model, Service, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageServices))
                return AccessDeniedView();

            //try to get a Service item with the specified id
            var Service = _serviceService.GetServiceById(id);
            if (Service == null)
                return RedirectToAction("List");

            _serviceService.DeleteService(Service);

            //activity log
            _customerActivityService.InsertActivity("DeleteService",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteService"), Service.Id), Service);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Services.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
        #endregion
    }
}