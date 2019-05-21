using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Services;
using Nop.Core.Html;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Services;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the Service model factory implementation
    /// </summary>
    public partial class ServiceModelFactory : IServiceModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IServiceService _serviceService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public ServiceModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            IServiceService ServiceService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IServiceCategoryService serviceCategoryService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._localizationService = localizationService;
            this._serviceService = ServiceService;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._storeService = storeService;
            this._serviceCategoryService = serviceCategoryService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare Service content model
        /// </summary>
        /// <param name="ServiceContentModel">Service content model</param>
        /// <param name="filterByServiceId">Filter by Service item ID</param>
        /// <returns>Service content model</returns>
        public virtual ServiceContentModel PrepareServiceContentModel(ServiceContentModel ServiceContentModel, int? filterByServiceId)
        {
            if (ServiceContentModel == null)
                throw new ArgumentNullException(nameof(ServiceContentModel));

            //prepare nested search models
            PrepareServiceSearchModel(ServiceContentModel.Services);
            return ServiceContentModel;
        }

        /// <summary>
        /// Prepare Service item search model
        /// </summary>
        /// <param name="searchModel">Service item search model</param>
        /// <returns>Service item search model</returns>
        public virtual ServiceSearchModel PrepareServiceSearchModel(ServiceSearchModel searchModel)
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
        /// Prepare paged Service item list model
        /// </summary>
        /// <param name="searchModel">Service item search model</param>
        /// <returns>Service item list model</returns>
        public virtual ServiceListModel PrepareServiceListModel(ServiceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get Service items
            var Services = _serviceService.GetAllService(showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ServiceListModel
            {
                Data = Services.Select(Service =>
                {
                    //fill in model values from the entity
                    var ServiceModel = Service.ToModel<ServiceModel>();

                    //little performance optimization: ensure that "Full" is not returned
                    ServiceModel.Full = string.Empty;
                    return ServiceModel;
                }),
                Total = Services.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare Service item model
        /// </summary>
        /// <param name="model">Service item model</param>
        /// <param name="Service">Service item</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Service item model</returns>
        public virtual ServiceModel PrepareServiceModel(ServiceModel model, Service Service, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (Service != null)
            {
                model = model ?? Service.ToModel<ServiceModel>();

                if (!excludeProperties)
                {
                    model.SelectedServiceCategoryIds = _serviceCategoryService.GetServiceCategoryMappingByServiceId(Service.Id, true)
                        .Select(productCategory => productCategory.CategoryId).ToList();
                }
            }

            //set default values for the new model
            if (Service == null)
            {
                model.Published = true;
            }

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, Service, excludeProperties);
            _baseAdminModelFactory.PrepareServiceCategories(model.AvailableServiceCategories, false);
            foreach (var categoryItem in model.AvailableServiceCategories)
            {
                categoryItem.Selected = int.TryParse(categoryItem.Value, out var categoryId)
                    && model.SelectedServiceCategoryIds.Contains(categoryId);
            }
            return model;
        }
        #endregion
    }
}