using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Banners;
using Nop.Services.Banners;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Factories
{
    public class BannerModelFactory : IBannerModelFactory
    {
        #region Fields
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;

        #endregion
        #region Ctor
        public BannerModelFactory(IBannerService bannerService, 
            IPictureService pictureService, 
            IStoreContext storeContext,
            IStaticCacheManager cacheManager)
        {
            this._bannerService = bannerService;
            this._pictureService = pictureService;
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
        }
        #endregion
        #region Method

        #endregion
        public IList<BannerModel> PrepareBannerModel(int storeId, int type, int categoryId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.BANNER_KEY, _storeContext.CurrentStore.Id,type,categoryId);
            var cacheModel= _cacheManager.Get(cacheKey, () => {
                var banners = _bannerService.GetAllBanners(storeId, type, categoryId);
                return banners.Select(b =>
                {
                    var picture = _pictureService.GetPictureById(b.PictureId);
                    var bannerModel = new BannerModel
                    {
                        Caption = b.Caption,
                        CategoryId = b.CategoryId,
                        DisplayOrder = b.DisplayOrder,
                        Id = b.Id,
                        PictureId = b.PictureId,
                        ImageUrl = _pictureService.GetPictureUrl(picture),
                        Url = b.Url,
                        Title = b.Title,
                        ShowCaption = b.ShowCaption
                    };
                    return bannerModel;
                }).ToList();
            });
            return cacheModel;
        }
    }
}
