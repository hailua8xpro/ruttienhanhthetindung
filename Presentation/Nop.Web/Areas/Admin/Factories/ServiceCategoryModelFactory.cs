using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Services;
using Nop.Services.Services;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class ServiceCategoryModelFactory : IServiceCategoryModelFactory
    {
        #region Fields
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IServiceService _serviceService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        #endregion
        #region Ctor
        public ServiceCategoryModelFactory(
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            IServiceCategoryService serviceCategoryService,
            IServiceService serviceService)
        {
            this._aclSupportedModelFactory = aclSupportedModelFactory;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._discountSupportedModelFactory = discountSupportedModelFactory;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._urlRecordService = urlRecordService;
            this._serviceCategoryService = serviceCategoryService;
            this._serviceService = serviceService;
        }


        #endregion
        #region Utilities

        /// <summary>
        /// Prepare category product search model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category product search model</returns>
        protected virtual CategoryServiceSearchModel PrepareCategoryServiceSearchModel(CategoryServiceSearchModel searchModel, ServiceCategory category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            searchModel.CategoryId = category.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion
        #region Methods

        public ServiceCategoryListModel PrepareCategoryListModel(ServiceCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var categories = _serviceCategoryService.GetAllServiceCategories(categoryName: searchModel.SearchCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new ServiceCategoryListModel
            {
                Data = categories.Select(category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<ServiceCategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = _serviceCategoryService.GetFormattedBreadCrumb(category);

                    return categoryModel;
                }),
                Total = categories.TotalCount
            };

            return model;
        }
        public ServiceCategorySearchModel PrepareServiceCategorySearchModel(ServiceCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        public virtual ServiceCategoryModel PrepareServiceCategoryModel(ServiceCategoryModel model, ServiceCategory category, bool excludeProperties = false)
        {
            Action<ServiceCategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                model = model ?? category.ToModel<ServiceCategoryModel>();

                //prepare nested search model
                PrepareCategoryServiceSearchModel(model.CategoryServiceSearchModel, category);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(category, entity => entity.Name, languageId, false, false);
                    locale.Description = _localizationService.GetLocalized(category, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = _localizationService.GetLocalized(category, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = _localizationService.GetLocalized(category, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = _localizationService.GetLocalized(category, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = _urlRecordService.GetSeName(category, languageId, false, false);
                };
            }

            //set default values for the new model
            if (category == null)
            {
                model.Published = true;
                model.IncludeInTopMenu = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);


            //prepare available parent categories
            _baseAdminModelFactory.PrepareServiceCategories(model.AvailableCategories,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"));
            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, category, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, category, excludeProperties);

            return model;
        }

        public virtual ServiceCategoryMappingListModel PrepareCategoryServiceListModel(CategoryServiceSearchModel searchModel, ServiceCategory category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //get product categories
            var ServiceCategories = _serviceCategoryService.GetServiceCategoryMappingByCategoryId(category.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new ServiceCategoryMappingListModel
            {
                //fill in model values from the entity
                Data = ServiceCategories.Select(ServiceCategory => new ServiceCategoryMappingModel
                {
                    Id = ServiceCategory.Id,
                    CategoryId = ServiceCategory.CategoryId,
                    ServiceId = ServiceCategory.ServiceId,
                    Name = _serviceService.GetServiceById(ServiceCategory.ServiceId)?.Name,

                }),
                Total = ServiceCategories.TotalCount
            };

            return model;
        }

        public AddServiceToServiceCategorySearchModel PrepareAddServiceToServiceCategorySearchModel(AddServiceToServiceCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareServiceCategories(searchModel.AvailableServiceCategories);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public AddServiceToServiceCategoryListModel PrepareAddServiceToServiceCategoryListModel(AddServiceToServiceCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var lstService = _serviceService.SearchService(serviceCategoryId:0, showHidden: true,
                storeId: searchModel.SearchStoreId,
                keywords: searchModel.SearchServiceName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddServiceToServiceCategoryListModel
            {
                //fill in model values from the entity
                Data = lstService.Select(product => product.ToModel<ServiceModel>()),
                Total = lstService.TotalCount
            };

            //return model;
            return model;
        }
        #endregion

    }
}
