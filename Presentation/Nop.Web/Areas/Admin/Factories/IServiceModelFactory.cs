using Nop.Core.Domain.Services;
using Nop.Web.Areas.Admin.Models.Services;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the Service model factory
    /// </summary>
    public partial interface IServiceModelFactory
    {
        /// <summary>
        /// Prepare Service content model
        /// </summary>
        /// <param name="ServiceContentModel">Service content model</param>
        /// <param name="filterByServiceId">Filter by Service item ID</param>
        /// <returns>Service content model</returns>
        ServiceContentModel PrepareServiceContentModel(ServiceContentModel ServiceContentModel, int? filterByServiceId);
        
        /// <summary>
        /// Prepare Service item search model
        /// </summary>
        /// <param name="searchModel">Service item search model</param>
        /// <returns>Service item search model</returns>
        ServiceSearchModel PrepareServiceSearchModel(ServiceSearchModel searchModel);

        /// <summary>
        /// Prepare paged Service item list model
        /// </summary>
        /// <param name="searchModel">Service item search model</param>
        /// <returns>Service item list model</returns>
        ServiceListModel PrepareServiceListModel(ServiceSearchModel searchModel);

        /// <summary>
        /// Prepare Service item model
        /// </summary>
        /// <param name="model">Service item model</param>
        /// <param name="Service">Service item</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Service item model</returns>
        ServiceModel PrepareServiceModel(ServiceModel model, Service Service, bool excludeProperties = false);
    }
}