using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Services
{
    /// <summary>
    /// Represents a Service content model
    /// </summary>
    public partial class ServiceContentModel : BaseNopModel
    {
        #region Ctor

        public ServiceContentModel()
        {
            Services = new ServiceSearchModel();
        }

        #endregion

        #region Properties

        public ServiceSearchModel Services { get; set; }
        #endregion
    }
}
