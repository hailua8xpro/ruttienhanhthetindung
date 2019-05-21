using System.Collections.Generic;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Banners;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Areas.Admin.Validators.Banners;

namespace Nop.Web.Areas.Admin.Models.Banners
{
    [Validator(typeof(BannerValidator))]
    public partial class BannerModel: BaseNopEntityModel, IStoreMappingSupportedModel,
        ILocalizedModel<BannerLocalizedModel>
    {
        #region Ctor
        public BannerModel()
        {
            Locales = new List<BannerLocalizedModel>();
            SelectedStoreIds = new List<int>();
            AvailableBannerTypes = new List<SelectListItem>(); ;
            AvailableCategoryEntity = new List<SelectListItem>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Type")]
        public int Type { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.CategoryId")]
        public int CategoryId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Url")]
        public string Url { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Title")]
        public string Title { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Caption")]
        public string Caption { get; set; }
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Picture")]
        public int PictureId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.ShowCaption")]
        public bool ShowCaption { get; set; }
        [NopResourceDisplayName("Admin.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Deleted")]
        public bool Deleted { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.PictureThumbnailUrl")]
        public string ImageUrl { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Type")]
        public IList<BannerLocalizedModel> Locales { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableBannerTypes { get; set; }
        public IList<SelectListItem> AvailableCategoryEntity { get; set; }
        #endregion
    }
    public partial class BannerLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Caption")]
        public string Caption { get; set; }
    }
}
