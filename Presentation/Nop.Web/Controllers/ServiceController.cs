using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Services;
using Nop.Core.Domain.Security;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Services;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Rss;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Services;

namespace Nop.Web.Controllers
{
    public class ServiceController : BasePublicController
    {
        #region Fields
        private readonly IServiceCategoryModelFactory _ServiceCategoryModelFactory;
        private readonly IAclService _aclService;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IServiceItemModelFactory _serviceModelFactory;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _categoryService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ServiceSettings _serviceSettings;

        #endregion

        #region Ctor

        public ServiceController(CaptchaSettings captchaSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IServiceItemModelFactory serviceModelFactory,
            IServiceService ServiceService,
            IServiceCategoryService categoryService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ServiceSettings ServiceSettings,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IServiceCategoryModelFactory ServiceCategoryModelFactory)
        {
            this._captchaSettings = captchaSettings;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._serviceModelFactory = serviceModelFactory;
            this._serviceService = ServiceService;
            this._categoryService = categoryService;
            this._permissionService = permissionService;
            this._storeContext = storeContext;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._serviceSettings = ServiceSettings;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._ServiceCategoryModelFactory = ServiceCategoryModelFactory;
        }

        #endregion

        #region Methods

        public virtual IActionResult List(ServicePagingFilteringModel command)
        {
            if (!_serviceSettings.Enabled)
                return RedirectToRoute("HomePage");

            var model = _serviceModelFactory.PrepareServiceItemListModel(command);
            return View(model);
        }
        public virtual IActionResult Category(int categoryId, ServicePagingFilteringModel command)
        {
            if (!_serviceSettings.Enabled)
                return RedirectToRoute("HomePage");

            var category = _categoryService.GetServiceCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageServices))
                DisplayEditLink(Url.Action("Edit", "ServiceCategory", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = _ServiceCategoryModelFactory.PrepareServiceCategoryModel(category, command);
            return View(model);
        }
        public virtual IActionResult ServiceItem(int ServiceId)
        {
            if (!_serviceSettings.Enabled)
                return RedirectToRoute("HomePage");

            var Service = _serviceService.GetServiceById(ServiceId);
            if (Service == null)
                return RedirectToRoute("HomePage");

            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageServices);
            //access to Service preview
            if (!Service.Published)
                return RedirectToRoute("HomePage");

            var model = new ServiceItemModel();
            model = _serviceModelFactory.PrepareServiceItemModel(model, Service);

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("Edit", "Service", new { id = Service.Id, area = AreaNames.Admin }));

            return View(model);
        }
        #endregion
    }
}