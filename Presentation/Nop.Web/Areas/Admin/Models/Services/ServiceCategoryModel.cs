using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Services;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Services
{
    /// <summary>
    /// Represents a Service category model
    /// </summary>
    [Validator(typeof(ServiceCategoryValidator))]
    public class ServiceCategoryModel : BaseNopEntityModel, IAclSupportedModel,
        ILocalizedModel<ServiceCategoryLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public ServiceCategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }

            Locales = new List<ServiceCategoryLocalizedModel>();
            AvailableCategories = new List<SelectListItem>();
            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            ServiceCategorySearchModel = new ServiceCategorySearchModel();
            CategoryServiceSearchModel = new CategoryServiceSearchModel();
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

        public IList<ServiceCategoryLocalizedModel> Locales { get; set; }

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

        public ServiceCategorySearchModel ServiceCategorySearchModel { get; set; }
        public CategoryServiceSearchModel CategoryServiceSearchModel { get; set; }

        #endregion
    }

    public partial class ServiceCategoryLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Service.Categories.Fields.SeName")]
        public string SeName { get; set; }
    }
}
