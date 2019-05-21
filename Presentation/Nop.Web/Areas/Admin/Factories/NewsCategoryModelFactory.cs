using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.News;
using Nop.Services.News;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class NewsCategoryModelFactory : INewsCategoryModelFactory
    {
        #region Fields
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly INewsService _newsService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        #endregion
        #region Ctor
        public NewsCategoryModelFactory(
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            INewsCategoryService newsCategoryService,
            INewsService newsService)
        {
            this._aclSupportedModelFactory = aclSupportedModelFactory;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._discountSupportedModelFactory = discountSupportedModelFactory;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._urlRecordService = urlRecordService;
            this._newsCategoryService = newsCategoryService;
            this._newsService = newsService;
        }


        #endregion
        #region Utilities

        /// <summary>
        /// Prepare category product search model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category product search model</returns>
        protected virtual CategoryNewsSearchModel PrepareCategoryNewsSearchModel(CategoryNewsSearchModel searchModel, NewsCategory category)
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

        public NewsCategoryListModel PrepareCategoryListModel(NewsCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var categories = _newsCategoryService.GetAllNewsCategories(categoryName: searchModel.SearchCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new NewsCategoryListModel
            {
                Data = categories.Select(category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<NewsCategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = _newsCategoryService.GetFormattedBreadCrumb(category);

                    return categoryModel;
                }),
                Total = categories.TotalCount
            };

            return model;
        }
        public NewsCategorySearchModel PrepareNewsCategorySearchModel(NewsCategorySearchModel searchModel)
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
        public virtual NewsCategoryModel PrepareNewsCategoryModel(NewsCategoryModel model, NewsCategory category, bool excludeProperties = false)
        {
            Action<NewsCategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                model = model ?? category.ToModel<NewsCategoryModel>();

                //prepare nested search model
                PrepareCategoryNewsSearchModel(model.CategoryNewsSearchModel, category);

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
            _baseAdminModelFactory.PrepareNewsCategories(model.AvailableCategories,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"));
            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, category, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, category, excludeProperties);

            return model;
        }

        public virtual NewsCategoryMappingListModel PrepareCategoryNewsListModel(CategoryNewsSearchModel searchModel, NewsCategory category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //get product categories
            var newsCategories = _newsCategoryService.GetNewsCategoryMappingByCategoryId(category.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new NewsCategoryMappingListModel
            {
                //fill in model values from the entity
                Data = newsCategories.Select(newsCategory => new NewsCategoryMappingModel
                {
                    Id = newsCategory.Id,
                    CategoryId = newsCategory.CategoryId,
                    NewsId = newsCategory.NewsId,
                    Title = _newsService.GetNewsById(newsCategory.NewsId)?.Title,

                }),
                Total = newsCategories.TotalCount
            };

            return model;
        }

        public AddNewsToNewsCategorySearchModel PrepareAddNewsToNewsCategorySearchModel(AddNewsToNewsCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareNewsCategories(searchModel.AvailableNewsCategories);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public AddNewsToNewsCategoryListModel PrepareAddNewsToNewsCategoryListModel(AddNewsToNewsCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var lstnews = _newsService.SearchNews(newsCategoryId:0, showHidden: true,
                storeId: searchModel.SearchStoreId,
                keywords: searchModel.SearchNewsTitle,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddNewsToNewsCategoryListModel
            {
                //fill in model values from the entity
                Data = lstnews.Select(product => product.ToModel<NewsItemModel>()),
                Total = lstnews.TotalCount
            };

            //return model;
            return model;
        }
        #endregion

    }
}
