using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Banners
{
   public static class NopBannerDefaults
    {
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string BannersPatternCacheKey => "Nop.banner.";
        public static string BannersByCategoryCacheKey => "Nop.banner.bycategory-{0}-{1}-{2}";
    }
}
