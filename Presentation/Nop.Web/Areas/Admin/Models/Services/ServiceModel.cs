using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Services;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Services
{
    /// <summary>
    /// Represents a Service item model
    /// </summary>
    [Validator(typeof(ServiceValidator))]
    public partial class ServiceModel : BaseNopEntityModel, IStoreMappingSupportedModel
    {
        #region Ctor

        public ServiceModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableServiceCategories = new List<SelectListItem>();
            SelectedServiceCategoryIds = new List<int>();
            Locales = new List<ServiceLocalizedModel>();

        }

        #endregion

        #region Properties
        public int LanguageId { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Name")]
        public string Name { get; set; }
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Picture")]
        public int PictureId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Short")]
        public string Short { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Full")]
        public string Full { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.SeName")]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Services.Item.Field.Categories")]
        public IList<int> SelectedServiceCategoryIds { get; set; }
        public IList<SelectListItem> AvailableServiceCategories { get; set; }
        public IList<ServiceLocalizedModel> Locales { get; set; }

        #endregion
    }
    public partial class ServiceLocalizedModel : ILocalizedLocaleModel
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
