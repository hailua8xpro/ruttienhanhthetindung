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
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    public class NewsCategoryModelFactory : INewsCategoryModelFactory
    {
        #region fields
        private readonly ILocalizationService _localizationService;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly INewsModelFactory _newsModelFactory;
        private readonly INewsService _newsService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly NewsSettings _newsSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;

        #endregion
        #region ctor
        public NewsCategoryModelFactory(ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            NewsSettings newsSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStaticCacheManager cacheManager,
            INewsCategoryService newsCategoryService,
            IWebHelper webHelper,
            INewsService newsService,
            INewsModelFactory newsModelFactory,
            MediaSettings mediaSettings
            )
        {
            this._localizationService = localizationService;
            this._urlRecordService = urlRecordService;
            this._newsSettings = newsSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._newsCategoryService = newsCategoryService;
            this._webHelper = webHelper;
            this._newsService = newsService;
            this._newsModelFactory = newsModelFactory;
            this._mediaSettings = mediaSettings;
            this._cacheManager = cacheManager;
        }
        #endregion
        #region method
        public NewsCategoryModel PrepareNewsCategoryModel(NewsCategory category, NewsPagingFilteringModel command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;
            var model = new NewsCategoryModel
            {
                Id = category.Id,
                Name = _localizationService.GetLocalized(category, x => x.Name),
                Description = _localizationService.GetLocalized(category, x => x.Description),
                MetaKeywords = _localizationService.GetLocalized(category, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(category, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(category, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(category),
            };
            //category breadcrumb
            if (_newsSettings.NewsCategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.NEWSCATEGORY_BREADCRUMB_KEY,
                    category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                model.CategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                    _newsCategoryService.GetNewsCategoryBreadCrumb(category).Select(catBr => new NewsCategoryModel
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
            if (_newsSettings.ShowNewsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(_newsCategoryService.GetChildCategoryIds(category.Id, _storeContext.CurrentStore.Id));
            }
            //products
            var newslist = _newsService.SearchNews(
                showHidden:false,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            var itemListModel = new NewsItemListModel();
            itemListModel.WorkingLanguageId = _workContext.WorkingLanguage.Id;
            itemListModel.PagingFilteringContext.LoadPagedList(newslist);

            itemListModel.NewsItems = newslist
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                _newsModelFactory.PrepareNewsItemModel(newsModel, x, false);
                    return newsModel;
                })
                .ToList();
            model.NewsItemListModel=itemListModel;
            return model;
        }
       public NewsCategoryNavigationModel PrepareNewsCategoryNavigationModel(int currentCategoryId, int currentNewsId)
        {
            //get active category
            var activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentNewsId > 0)
            {
                //product details page
                var productCategories = _newsCategoryService.GetNewsCategoryMappingByNewsId(currentNewsId);
                if (productCategories.Any())
                    activeCategoryId = productCategories[0].CategoryId;
            }

            var cachedCategoriesModel = PrepareCategorySimpleModels();
            var model = new NewsCategoryNavigationModel
            {
                CurrentNewsCategoryId = activeCategoryId,
                NewsCategories = cachedCategoriesModel
            };

            return model;
        }
        public virtual List<NewsCategorySimpleModel> PrepareCategorySimpleModels()
        {
            //load and cache them
            var cacheKey = string.Format(ModelCacheEventConsumer.NEWS_CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        public virtual List<NewsCategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId, bool loadSubCategories = true)
        {
            var result = new List<NewsCategorySimpleModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var allCategories = _newsCategoryService.GetAllNewsCategories(storeId: _storeContext.CurrentStore.Id);
            var categories = allCategories.Where(c => rootCategoryId==0 || c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new NewsCategorySimpleModel
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
        #endregion

    }
}
