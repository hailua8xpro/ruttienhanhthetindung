using System.Collections.Generic;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Testimonials
{
    public class TestimonialModel: BaseNopEntityModel, IStoreMappingSupportedModel,
        ILocalizedModel<TestimonialLocalizedModel>
    {
        #region ctor
        public TestimonialModel()
        {
            Locales = new List<TestimonialLocalizedModel>();
            SelectedStoreIds = new List<int>();
        }
        #endregion
        #region properties
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.FullName")]

        public string FullName { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Description")]
        public string Description { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.FullDescription")]
        public string FullDescription { get; set; }
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Picture")]
        public int PictureId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.PictureThumbnailUrl")]
        public string ImageUrl { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Deleted")]

        public bool Deleted { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [NopResourceDisplayName("Admin.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<TestimonialLocalizedModel> Locales { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        #endregion
    }
    public partial class TestimonialLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Description")]
        public string Description { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.FullDescription")]
        public string FullDescription { get; set; }
    }
}

