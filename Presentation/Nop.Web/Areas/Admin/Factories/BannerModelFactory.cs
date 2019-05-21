using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Banners;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Core.Domain.Banners;
using Nop.Services.Media;

namespace Nop.Web.Areas.Admin.Factories
{
    public class BannerModelFactory : IBannerModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IBannerService _bannerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;


        #endregion

        #region Ctor

        public BannerModelFactory(
            IBaseAdminModelFactory baseAdminModelFactory,
            IBannerService bannerService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            IPictureService pictureService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._bannerService = bannerService;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
        }

        #endregion
        #region Utilities
        
        #endregion
        #region Methods
        public BannerSearchModel PrepareBannerSearchModel(BannerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);
            _baseAdminModelFactory.PrepareBannerCategoryEntity(searchModel.AvailableCategoryEntity,true);
            //prepare page parameters
            searchModel.SetGridPageSize();
            return searchModel;
        }

        public BannerListModel PrepareBannerListModel(BannerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var banners = _bannerService.GetAllBanners(showHidden: true, type
                :searchModel.Type,
                storeId: searchModel.SearchStoreId,
                categoryId:searchModel.CategoryId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new BannerListModel
            {
                //fill in model values from the entity
                Data = banners.Select(banner => {
                    var bannermodel = banner.ToModel<BannerModel>();
                    bannermodel.ImageUrl = _pictureService.GetPictureUrl(bannermodel.PictureId, 120);
                    return bannermodel;
                }),
                Total = banners.TotalCount
            };

            return model;
        }
        public virtual BannerModel PrepareBannerModel(BannerModel model,
            Banner banner, bool excludeProperties = false)
        {
            Action<BannerLocalizedModel, int> localizedModelConfiguration = null;

            if (banner != null)
            {
                //fill in model values from the entity
                model = model ?? banner.ToModel<BannerModel>();
                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Title = _localizationService.GetLocalized(banner, entity => entity.Title, languageId, false, false);
                    locale.Caption = _localizationService.GetLocalized(banner, entity => entity.Caption, languageId, false, false);
                };
            }

            //set default values for the new model
            if (banner == null)
            {
                model.Published = true;
                model.Type = 0;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, banner, excludeProperties);
            _baseAdminModelFactory.PrepareStores(model.AvailableStores);
            _baseAdminModelFactory.PrepareBannerCategoryEntity(model.AvailableCategoryEntity, true,null,(BannerType)model.Type);

            return model;
        }
        #endregion



    }
}
