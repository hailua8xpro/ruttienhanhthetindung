using System;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;


namespace Nop.Core.Domain.Testimonial
{
   public partial class Testimonial : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public int PictureId { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public bool LimitedToStores { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
