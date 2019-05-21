using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.News;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
//using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class NewsCategoryController : BaseAdminController
    {
        #region Fields
        private readonly IAclService _aclService;
        private readonly INewsCategoryModelFactory _newsCategoryModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly INewsService _newsService;
        #endregion
        #region Ctor
        public NewsCategoryController(IAclService aclService,
            INewsCategoryModelFactory categoryModelFactory,
            INewsCategoryService newsCategoryService,
            INewsService newsService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            this._aclService = aclService;
            this._newsCategoryService = newsCategoryService;
            this._newsService = newsService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._newsCategoryModelFactory = categoryModelFactory;
        }
        #endregion
        #region Utilities
        protected virtual void UpdateLocales(NewsCategory category, NewsCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(category, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(NewsCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }
        protected virtual void SaveCategoryAcl(NewsCategory category, NewsCategoryModel model)
        {
            category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(category);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(category, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }
        protected virtual void SaveStoreMappings(NewsCategory category, NewsCategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(category, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }
        #endregion
        #region List
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategorySearchModel(new NewsCategorySearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult List(NewsCategorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareCategoryListModel(searchModel);

            return Json(model);
        }
        [HttpPost]
        public virtual IActionResult NewsList(CategoryNewsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(searchModel.CategoryId)
                ?? throw new ArgumentException("No category found with the specified id");

            //prepare model
            var model = _newsCategoryModelFactory.PrepareCategoryNewsListModel(searchModel, category);

            return Json(model);
        }
        #endregion
        #region Create / Edit / Delete
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategoryModel(new NewsCategoryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<NewsCategory>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.InsertNewsCategory(category);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(category, model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //locales
                UpdateLocales(category, model);


                _newsCategoryService.UpdateNewsCategory(category);

                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL (customer roles)
                SaveCategoryAcl(category, model);

                //stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCategory",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCategory"), category.Name), category);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _newsCategoryModelFactory.PrepareNewsCategoryModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategoryModel(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(model.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;

                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.UpdateNewsCategory(category);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(category, model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //locales
                UpdateLocales(category, model);
                _newsCategoryService.UpdateNewsCategory(category);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL
                SaveCategoryAcl(category, model);

                //stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("EditNewsCategory",
                    string.Format(_localizationService.GetResource("ActivityLog.EditNewsCategory"), category.Name), category);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsCategories.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _newsCategoryModelFactory.PrepareNewsCategoryModel(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        public virtual IActionResult NewsDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a product category with the specified id
            var newsCategory = _newsCategoryService.GetNewsCategoryById(id)
                ?? throw new ArgumentException("No news category mapping found with the specified id", nameof(id));

            _newsCategoryService.DeleteNewsCategory(newsCategory);

            return new NullJsonResult();
        }
        #endregion

        #region news
        public virtual IActionResult NewsAddPopup(int newsCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareAddNewsToNewsCategorySearchModel(new AddNewsToNewsCategorySearchModel());

            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult NewsAddPopup(AddNewsToNewsCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = _newsService.GetNewsByIds(model.SelectedNewsIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingNewsCategoryMapping = _newsCategoryService.GetNewsCategoryMappingByCategoryId(model.NewsCategoryId, showHidden: true);
                foreach (var news in selectedProducts)
                {
                    //whether product category with such parameters already exists
                    if (_newsCategoryService.FindNewsCategoryMapping(existingNewsCategoryMapping, news.Id, model.NewsCategoryId) != null)
                        continue;

                    //insert the new product category mapping
                    _newsCategoryService.InsertNewsCategoryMapping(new NewsCategoryMapping
                    {
                        CategoryId = model.NewsCategoryId,
                        NewsId = news.Id,
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddNewsToNewsCategorySearchModel());
        }

        [HttpPost]
        public virtual IActionResult NewsAddPopupList(AddNewsToNewsCategorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareAddNewsToNewsCategoryListModel(searchModel);

            return Json(model);
        }
        #endregion

    }
}