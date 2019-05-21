﻿using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the news model factory
    /// </summary>
    public partial class NewsModelFactory : INewsModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INewsService _newsService;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public NewsModelFactory(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            IPictureService pictureService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            INewsCategoryService newsCategoryService,
            ILocalizationService localizationService)
        {
            this._captchaSettings = captchaSettings;
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._genericAttributeService = genericAttributeService;
            this._newsService = newsService;
            this._pictureService = pictureService;
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._newsSettings = newsSettings;
            this._newsCategoryService = newsCategoryService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the news comment model
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>News comment model</returns>
        public virtual NewsCommentModel PrepareNewsCommentModel(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException(nameof(newsComment));

            var model = new NewsCommentModel
            {
                Id = newsComment.Id,
                CustomerId = newsComment.CustomerId,
                CustomerName = _customerService.FormatUserName(newsComment.Customer),
                CommentTitle = newsComment.CommentTitle,
                CommentText = newsComment.CommentText,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && newsComment.Customer != null && !newsComment.Customer.IsGuest(),
            };
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                    _genericAttributeService.GetAttribute<int>(newsComment.Customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        /// <summary>
        /// Prepare the news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="prepareComments">Whether to prepare news comment models</param>
        /// <returns>News item model</returns>
        public virtual NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            model.Id = newsItem.Id;
            model.MetaTitle = newsItem.MetaTitle;
            model.MetaDescription = newsItem.MetaDescription;
            model.MetaKeywords = newsItem.MetaKeywords;
            model.SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = newsItem.Title;
            model.Short = newsItem.Short;
            model.Full = newsItem.Full;
            model.AllowComments = newsItem.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;

            //number of news comments
            var storeId = _newsSettings.ShowNewsCommentsPerStore ? _storeContext.CurrentStore.Id : 0;
            var cacheKey = string.Format(ModelCacheEventConsumer.NEWS_COMMENTS_NUMBER_KEY, newsItem.Id, storeId, true);
            model.NumberOfComments = _cacheManager.Get(cacheKey, () => _newsService.GetNewsCommentsCount(newsItem, storeId, true));

            if (prepareComments)
            {
                var newsComments = newsItem.NewsComments.Where(comment => comment.IsApproved);
                if (_newsSettings.ShowNewsCommentsPerStore)
                    newsComments = newsComments.Where(comment => comment.StoreId == _storeContext.CurrentStore.Id);

                foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
                {
                    var commentModel = PrepareNewsCommentModel(nc);
                    model.Comments.Add(commentModel);
                }
            }
            var newsItemPictureCacheKey = string.Format(ModelCacheEventConsumer.NEWSITEM_PICTURE_MODEL_KEY, model.Id, pictureSize, _storeContext.CurrentStore.Id);

            model.PictureModel = _cacheManager.Get(newsItemPictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(newsItem.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = model.Title,
                    AlternateText = model.Title
                };
                return pictureModel;
            });
            var newsItemCategoryKey = string.Format(ModelCacheEventConsumer.NEWSITEMS_CATEGORY_KEY, model.Id, _workContext.WorkingLanguage.Id);
            var newsCategory = _cacheManager.Get(newsItemCategoryKey, () => {
                return _newsCategoryService.GetFirstByNewsId(newsItem.Id);
            });
            if (newsCategory != null)
            {
                model.NewsCategoryName = newsCategory.Name;
                model.NewsCategorySeName = _urlRecordService.GetSeName(newsCategory);
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.NEWSCATEGORY_BREADCRUMB_KEY,
                    model.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                model.CategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                    _newsCategoryService.GetNewsCategoryBreadCrumb(newsCategory).Select(catBr => new NewsCategoryModel
                    {
                        Id = catBr.Id,
                        Name = _localizationService.GetLocalized(catBr, x => x.Name),
                        SeName = _urlRecordService.GetSeName(catBr)
                    })
                    .ToList()
                );
            }
            return model;
        }

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>Home page news items model</returns>
        public virtual HomePageNewsItemsModel PrepareHomePageNewsItemsModel()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_NEWSMODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id, 0, _newsSettings.MainPageNewsCount);
                return new HomePageNewsItemsModel
                {
                    WorkingLanguageId = _workContext.WorkingLanguage.Id,
                    NewsItems = newsItems
                        .Select(x =>
                        {
                            var newsModel = new NewsItemModel();
                            PrepareNewsItemModel(newsModel, x, false);
                            return newsModel;
                        })
                        .ToList()
                };
            });

            //"Comments" property of "NewsItemModel" object depends on the current customer.
            //Furthermore, we just don't need it for home page news. So let's reset it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (HomePageNewsItemsModel)cachedModel.Clone();
            foreach (var newsItemModel in model.NewsItems)
                newsItemModel.Comments.Clear();
            return model;
        }

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>News item list model</returns>
        public virtual NewsItemListModel PrepareNewsItemListModel(NewsPagingFilteringModel command)
        {
            var model = new NewsItemListModel
            {
                WorkingLanguageId = _workContext.WorkingLanguage.Id
            };

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id,
                command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newsItems);

            model.NewsItems = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    return newsModel;
                })
                .ToList();

            return model;
        }

        #endregion
    }
}