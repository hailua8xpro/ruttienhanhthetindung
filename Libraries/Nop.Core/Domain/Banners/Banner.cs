using System;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Banners
{
    public class Banner : BaseEntity, ILocalizedEntity,ISlugSupported, IStoreMappingSupported
    {
        public int Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public int PictureId { get; set; }
        public int CategoryId { get; set; }
        public bool Deleted { get; set; }
        public bool LimitedToStores { get; set; }
        public bool Published { get; set; }
        public bool ShowCaption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public BannerType BannerType
        {
            get => (BannerType)Type;
            set => Type = (int)value;
        }



    }
}
