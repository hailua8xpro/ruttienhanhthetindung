using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.News;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news category model
    /// </summary>
    [Validator(typeof(NewsCategoryValidator))]
    public class NewsCategoryModel : BaseNopEntityModel, IAclSupportedModel,
        ILocalizedModel<NewsCategoryLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public NewsCategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }

            Locales = new List<NewsCategoryLocalizedModel>();
            AvailableCategories = new List<SelectListItem>();
            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            NewsCategorySearchModel = new NewsCategorySearchModel();
            CategoryNewsSearchModel = new CategoryNewsSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<NewsCategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public NewsCategorySearchModel NewsCategorySearchModel { get; set; }
        public CategoryNewsSearchModel CategoryNewsSearchModel { get; set; }

        #endregion
    }

    public partial class NewsCategoryLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        public string SeName { get; set; }
    }
}
