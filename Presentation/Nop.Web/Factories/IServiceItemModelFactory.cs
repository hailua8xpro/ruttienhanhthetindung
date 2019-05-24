using Nop.Core.Domain.Services;
using Nop.Web.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public interface IServiceItemModelFactory
    {
        /// <summary>
        /// Prepare the Service item model
        /// </summary>
        /// <param name="model">Service item model</param>
        /// <param name="ServiceItem">Service item</param>
        /// <param name="prepareComments">Whether to prepare Service comment models</param>
        /// <returns>Service item model</returns>
        ServiceItemModel PrepareServiceItemModel(ServiceItemModel model, Service service);

        /// <summary>
        /// Prepare the home page Service items model
        /// </summary>
        /// <returns>Home page Service items model</returns>
        IList<ServiceSimpleModel> PrepareServiceSimpleModels();
        IList<ServiceItemModel> PrepareHomePageServiceItemsModel();
        IList<ServiceItemModel> PrepareOtherServiceItemsModel(int serviceId);
        /// <summary>
        /// Prepare the Service item list model
        /// </summary>
        /// <param name="command">Service paging filtering model</param>
        /// <returns>Service item list model</returns>
        ServiceItemListModel PrepareServiceItemListModel(ServicePagingFilteringModel command);

    }
}
