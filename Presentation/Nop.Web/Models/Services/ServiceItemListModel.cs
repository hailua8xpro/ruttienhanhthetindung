using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Services
{
    public class ServiceItemListModel
    {
        public ServiceItemListModel()
        {
            PagingFilteringContext = new ServicePagingFilteringModel();
            ServiceItems = new List<ServiceItemModel>();
        }

        public ServicePagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ServiceItemModel> ServiceItems { get; set; }
    }
}
