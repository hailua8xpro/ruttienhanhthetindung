using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Banners;
namespace Nop.Services.Banners
{
   public partial interface IBannerService
    {
        void DeleteBanner(Banner banner);
        Banner GetBannerById(int bannerId);
        IPagedList<Banner> GetAllBanners(int storeId = 0,int type=0,int categoryId=0,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        void InsertBanner(Banner banner);

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void UpdateBanner(Banner banner);
    }
}
