using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.News
{
    public static class NopNewsDefaults
    {
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesPatternCacheKey => "Nop.newscategory.";
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string NewsCategoriesPatternCacheKey => "Nop.newscategorymapping.";
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static string CategoriesAllCacheKey => "Nop.newscategory.all-{0}-{1}-{2}";
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string CategoriesByParentCategoryIdCacheKey => "Nop.newscategory.byparent-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static string CategoriesByIdCacheKey => "Nop.newscategory.id-{0}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static string CategoriesChildIdentifiersCacheKey => "Nop.newscategory.childidentifiers-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : category ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static string NewsCategoriesAllByCategoryIdCacheKey => "Nop.newscategorymapping.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : news ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string NewsCategoriesAllByNewsIdCacheKey => "Nop.newscategorymapping.allbynewsid-{0}-{1}-{2}-{3}";
    }
}
