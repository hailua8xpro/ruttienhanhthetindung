using Nop.Web.Areas.Admin.Models.Banners;
using System.Collections.Generic;

namespace Nop.Web.Factories
{
    public interface IBannerModelFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="type">page type ex: home, product category</param>
        /// <param name="categoryId">using when type is category</param>
        /// <returns></returns>
        IList<BannerModel> PrepareBannerModel(int storeId, int type, int categoryId);
    }
}
