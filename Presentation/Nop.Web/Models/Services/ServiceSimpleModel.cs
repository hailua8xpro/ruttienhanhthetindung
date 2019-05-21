using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Services
{
    public class ServiceSimpleModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public bool IncludeInTopMenu { get; set; }
    }
}
