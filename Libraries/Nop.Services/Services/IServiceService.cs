using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Services;

namespace Nop.Services.Services
{
    /// <summary>
    /// Service service interface
    /// </summary>
    public partial interface IServiceService
    {
        #region Service

        /// <summary>
        /// Deletes a Service
        /// </summary>
        /// <param name="Service">Service item</param>
        void DeleteService(Service service);

        /// <summary>
        /// Gets a Service
        /// </summary>
        /// <param name="ServiceId">The Service identifier</param>
        /// <returns>Service</returns>
        Service GetServiceById(int serviceId);

        /// <summary>
        /// Gets Service
        /// </summary>
        /// <param name="ServiceIds">The Service identifiers</param>
        /// <returns>Service</returns>
        IList<Service> GetServiceByIds(int[] serviceIds);

        /// <summary>
        /// Gets all Service
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Service items</returns>
        IPagedList<Service> GetAllService(int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        IPagedList<Service> SearchService(int serviceCategoryId = 0, string keywords = null, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        IPagedList<Service> SearchService(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int storeId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool showHidden = false,
            ProductSortingEnum orderBy = ProductSortingEnum.CreatedOn);
        /// <summary>
        /// Inserts a Service item
        /// </summary>
        /// <param name="Service">Service item</param>
        void InsertService(Service service);

        /// <summary>
        /// Updates the Service item
        /// </summary>
        /// <param name="Service">Service item</param>
        void UpdateService(Service service);
        #endregion

    }
}