using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Services;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.Services;
using Nop.Core.Domain.Services;

namespace Nop.Web.Factories
{
    public class ServiceCategoryModelFactory : IServiceCategoryModelFactory
    {
        #region field
        private readonly ILocalizationService _localizationService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IServiceItemModelFactory _serviceModelFactory;
        private readonly IServiceService _serviceService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ServiceSettings _serviceSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        #endregion
        #region ctor
        public ServiceCategoryModelFactory(ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            ServiceSettings serviceSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStaticCacheManager cacheManager,
            IServiceCategoryService serviceCategoryService,
            IWebHelper webHelper,
            IServiceService serviceService,
            IServiceItemModelFactory serviceModelFactory,
            MediaSettings mediaSettings)
        {
            this._localizationService = localizationService;
            this._urlRecordService = urlRecordService;
            this._serviceSettings = serviceSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._serviceCategoryService = serviceCategoryService;
            this._webHelper = webHelper;
            this._serviceService = serviceService;
            this._serviceModelFactory = serviceModelFactory;
            this._mediaSettings = mediaSettings;
            this._cacheManager = cacheManager;
        }
        #endregion
        public List<ServiceCategorySimpleModel> PrepareCategorySimpleModels()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.SERVICE_CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(0));
        }

        public List<ServiceCategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId, bool loadSubCategories = true)
        {
            var result = new List<ServiceCategorySimpleModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var allCategories = _serviceCategoryService.GetAllServiceCategories(storeId: _storeContext.CurrentStore.Id);
            var categories = allCategories.Where(c => rootCategoryId == 0 || c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new ServiceCategorySimpleModel
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name),
                    SeName = _urlRecordService.GetSeName(category),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };
                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        public ServiceCategoryModel PrepareServiceCategoryModel(ServiceCategory category, ServicePagingFilteringModel command)
        {
            {
                if (category == null)
                    throw new ArgumentNullException(nameof(category));
                if (command.PageSize <= 0) command.PageSize = _serviceSettings.ServicePageSize;
                if (command.PageNumber <= 0) command.PageNumber = 1;
                var model = new ServiceCategoryModel
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name),
                    Description = _localizationService.GetLocalized(category, x => x.Description),
                    MetaKeywords = _localizationService.GetLocalized(category, x => x.MetaKeywords),
                    MetaDescription = _localizationService.GetLocalized(category, x => x.MetaDescription),
                    MetaTitle = _localizationService.GetLocalized(category, x => x.MetaTitle),
                    SeName = _urlRecordService.GetSeName(category),
                };
                if (_serviceSettings.ServiceCategoryBreadcrumbEnabled)
                {
                    model.DisplayCategoryBreadcrumb = true;

                    var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.SERVICECATEGORY_BREADCRUMB_KEY,
                        category.Id,
                        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id,
                        _workContext.WorkingLanguage.Id);
                    model.CategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                        _serviceCategoryService.GetServiceCategoryBreadCrumb(category).Select(catBr => new ServiceCategoryModel
                        {
                            Id = catBr.Id,
                            Name = _localizationService.GetLocalized(catBr, x => x.Name),
                            SeName = _urlRecordService.GetSeName(catBr)
                        })
                        .ToList()
                    );
                }
                var categoryIds = new List<int>();
                categoryIds.Add(category.Id);
                if (_serviceSettings.ShowServicesFromSubcategories)
                {
                    //include subcategories
                    categoryIds.AddRange(_serviceCategoryService.GetChildCategoryIds(category.Id, _storeContext.CurrentStore.Id));
                }
                //products
                var serviceList = _serviceService.SearchService(
                    showHidden: false,
                    categoryIds: categoryIds,
                    storeId: _storeContext.CurrentStore.Id,
                    pageIndex: command.PageNumber - 1,
                    pageSize: command.PageSize);
                var itemListModel = new ServiceItemListModel();
                itemListModel.PagingFilteringContext.LoadPagedList(serviceList);

                itemListModel.ServiceItems = serviceList
                    .Select(x =>
                    {
                        var serviceModel = new ServiceItemModel();
                        _serviceModelFactory.PrepareServiceItemModel(serviceModel, x);
                        return serviceModel;
                    })
                    .ToList();
                model.ServiceItemListModel = itemListModel;
                return model;
            }
        }

        public ServiceCategoryNavigationModel PrepareServiceCategoryNavigationModel(int currentCategoryId, int currentServiceId)
        {
            //get active category
            var activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentServiceId > 0)
            {
                //product details page
                var serviceCategories = _serviceCategoryService.GetServiceCategoryMappingByServiceId(currentServiceId);
                if (serviceCategories.Any())
                    activeCategoryId = serviceCategories[0].CategoryId;
            }

            var cachedCategoriesModel = PrepareCategorySimpleModels();
            var model = new ServiceCategoryNavigationModel
            {
                CurrentServiceCategoryId = activeCategoryId,
                ServiceCategories = cachedCategoriesModel
            };

            return model;
        }
    }
}
