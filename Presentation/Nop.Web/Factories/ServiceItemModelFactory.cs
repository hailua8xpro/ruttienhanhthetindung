using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Services;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.Services;
using System.Collections.Generic;

namespace Nop.Web.Factories
{
    public class ServiceItemModelFactory : IServiceItemModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly ServiceSettings _serviceSettings;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public ServiceItemModelFactory(
            IDateTimeHelper dateTimeHelper,
            IServiceService ServiceService,
            IPictureService pictureService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            ServiceSettings ServiceSettings,
            IServiceCategoryService ServiceCategoryService,
            ILocalizationService localizationService)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._serviceService = ServiceService;
            this._pictureService = pictureService;
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._serviceSettings = ServiceSettings;
            this._serviceCategoryService = ServiceCategoryService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Prepare the Service item model
        /// </summary>
        /// <param name="model">Service item model</param>
        /// <param name="Service">Service item</param>
        /// <param name="prepareComments">Whether to prepare Service comment models</param>
        /// <returns>Service item model</returns>
        public virtual ServiceItemModel PrepareServiceItemModel(ServiceItemModel model, Service Service)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (Service == null)
                throw new ArgumentNullException(nameof(Service));
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            model.Id = Service.Id;
            model.MetaTitle = Service.MetaTitle;
            model.MetaDescription = Service.MetaDescription;
            model.MetaKeywords = Service.MetaKeywords;
            model.SeName = _urlRecordService.GetSeName(Service, _workContext.WorkingLanguage.Id, ensureTwoPublishedLanguages: false);
            model.Name = Service.Name;
            model.Short = Service.Short;
            model.Full = Service.Full;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(Service.CreatedOnUtc, DateTimeKind.Utc);
            //number of Service comments
            var storeId = _storeContext.CurrentStore.Id;
            var ServicePictureCacheKey = string.Format(ModelCacheEventConsumer.SERVICEITEM_PICTURE_MODEL_KEY, model.Id, pictureSize, _storeContext.CurrentStore.Id);

            model.PictureModel = _cacheManager.Get(ServicePictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(Service.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = model.Name,
                    AlternateText = model.Name
                };
                return pictureModel;
            });
            var ServiceCategoryKey = string.Format(ModelCacheEventConsumer.SERVICEITEM_CATEGORY_KEY, model.Id, _workContext.WorkingLanguage.Id);
            var ServiceCategory = _cacheManager.Get(ServiceCategoryKey, () =>
            {
                return _serviceCategoryService.GetFirstByServiceId(Service.Id);
            });
            if (ServiceCategory != null)
            {
                model.ServiceCategoryName = ServiceCategory.Name;
                model.ServiceCategorySeName = _urlRecordService.GetSeName(ServiceCategory);
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.NEWSCATEGORY_BREADCRUMB_KEY,
                    model.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                model.CategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                    _serviceCategoryService.GetServiceCategoryBreadCrumb(ServiceCategory).Select(catBr => new ServiceCategorySimpleModel
                    {
                        Id = catBr.Id,
                        Name = _localizationService.GetLocalized(catBr, x => x.Name),
                        SeName = _urlRecordService.GetSeName(catBr)
                    })
                    .ToList()
                );
            }
            return model;
        }

        /// <summary>
        /// Prepare the home page Service items model
        /// </summary>
        /// <returns>Home page Service items model</returns>
        public IList<ServiceItemModel> PrepareHomePageServiceItemsModel()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_SERVICESMODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var Services = _serviceService.GetAllService(_storeContext.CurrentStore.Id, 0);
                return Services
                        .Select(x =>
                        {
                            var ServiceItemModel = new ServiceItemModel();
                            PrepareServiceItemModel(ServiceItemModel, x);
                            return ServiceItemModel;
                        })
                        .ToList();
            });
        }

        public IList<ServiceItemModel> PrepareOtherServiceItemsModel(int serviceId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.SERVICEITEM_OTHERSERVICE_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id, serviceId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var Services = _serviceService.GetAllService(_storeContext.CurrentStore.Id, 0);
                return Services.Where(s => s.Id != serviceId)
                        .Select(x =>
                        {
                            var ServiceItemModel = new ServiceItemModel();
                            PrepareServiceItemModel(ServiceItemModel, x);
                            return ServiceItemModel;
                        })
                        .ToList();
            });
        }

        /// <summary>
        /// Prepare the Service item list model
        /// </summary>
        /// <param name="command">Service paging filtering model</param>
        /// <returns>Service item list model</returns>
        public virtual ServiceItemListModel PrepareServiceItemListModel(ServicePagingFilteringModel command)
        {
            var model = new ServiceItemListModel();

            if (command.PageSize <= 0) command.PageSize = _serviceSettings.ServicePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var Services = _serviceService.GetAllService(_storeContext.CurrentStore.Id,
                command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(Services);

            model.ServiceItems = Services
                .Select(x =>
                {
                    var ServiceItemModel = new ServiceItemModel();
                    PrepareServiceItemModel(ServiceItemModel, x);
                    return ServiceItemModel;
                })
                .ToList();

            return model;
        }
        public IList<ServiceSimpleModel> PrepareServiceSimpleModels()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.SERVICE_ALL_MODEL_KEY,
               _workContext.WorkingLanguage.Id,
               string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
               _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var Services = _serviceService.GetAllService(_storeContext.CurrentStore.Id, 0);
                return Services
                        .Select(service =>
                        {
                            var ServiceItemModel = new ServiceSimpleModel
                            {
                                Id = service.Id,
                                IncludeInTopMenu = service.IncludeInTopMenu,
                                Name = _localizationService.GetLocalized(service, x => x.Name),
                                SeName = _urlRecordService.GetSeName(service),
                            };

                            return ServiceItemModel;
                        })
                        .ToList();
            });
        }


        #endregion
    }
}
