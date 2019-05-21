using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Services;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Events;

namespace Nop.Services.Services
{
    /// <summary>
    /// Service service
    /// </summary>
    public partial class ServiceService : IServiceService
    {
        #region Fields
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<ServiceCategoryMapping> _serviceCategoryMappingRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        private readonly string _entityName;

        #endregion

        #region Ctor

        public ServiceService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IEventPublisher eventPublisher,
            IDataProvider dataProvider,
            IRepository<Service> serviceRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<ServiceCategoryMapping> serviceCategoryMappingRepository,
            IDbContext dbContext)
        {
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._dataProvider = dataProvider;
            this._serviceRepository = serviceRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._entityName = typeof(Service).Name;
            this._serviceCategoryMappingRepository = serviceCategoryMappingRepository;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Service

        /// <summary>
        /// Deletes a Service
        /// </summary>
        /// <param name="Service">Service item</param>
        public virtual void DeleteService(Service Service)
        {
            if (Service == null)
                throw new ArgumentNullException(nameof(Service));

            _serviceRepository.Delete(Service);

            //event notification
            _eventPublisher.EntityDeleted(Service);
        }

        /// <summary>
        /// Gets a Service
        /// </summary>
        /// <param name="ServiceId">The Service identifier</param>
        /// <returns>Service</returns>
        public virtual Service GetServiceById(int ServiceId)
        {
            if (ServiceId == 0)
                return null;

            return _serviceRepository.GetById(ServiceId);
        }

        /// <summary>
        /// Gets Service
        /// </summary>
        /// <param name="ServiceIds">The Service identifiers</param>
        /// <returns>Service</returns>
        public virtual IList<Service> GetServiceByIds(int[] ServiceIds)
        {
            var query = _serviceRepository.Table;
            return query.Where(p => ServiceIds.Contains(p.Id)).ToList();
        }

        /// <summary>
        /// Gets all Service
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Service items</returns>
        public virtual IPagedList<Service> GetAllService(int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _serviceRepository.Table;
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
            }
            //Store mapping
            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = n.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || storeId == sm.StoreId
                        select n;

            }
            query = query.OrderByDescending(n => n.CreatedOnUtc);
            var Service = new PagedList<Service>(query, pageIndex, pageSize);
            return Service;
        }
        public virtual IPagedList<Service> SearchService(int serviceCategoryId = 0, string keywords = null, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _serviceRepository.Table;
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
            }


            //Store mapping
            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = n.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || storeId == sm.StoreId
                        select n;

                query = query.Distinct();
            }
            if (serviceCategoryId > 0)
            {
                query = from n in query
                        join ncm in _serviceCategoryMappingRepository.Table
                        on n.Id equals ncm.ServiceId
                        select n;
            }
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(n => n.Name.Contains(keywords));
            }
            query = query.OrderByDescending(n => n.CreatedOnUtc);
            var Service = new PagedList<Service>(query, pageIndex, pageSize);
            return Service;
        }
        public virtual IPagedList<Service> SearchService(
          int pageIndex = 0,
          int pageSize = int.MaxValue,
          IList<int> categoryIds = null,
          int storeId = 0,
          string keywords = null,
          bool searchDescriptions = false,
          bool showHidden = false,
          ProductSortingEnum orderBy = ProductSortingEnum.CreatedOn)
        {
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //pass category identifiers as comma-delimited string
            var commaSeparatedCategoryIds = categoryIds == null ? string.Empty : string.Join(",", categoryIds);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare input parameters
            var pCategoryIds = _dataProvider.GetStringParameter("CategoryIds", commaSeparatedCategoryIds);
            var pStoreId = _dataProvider.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
            var pKeywords = _dataProvider.GetStringParameter("Keywords", keywords);
            var pSearchDescriptions = _dataProvider.GetBooleanParameter("SearchDescriptions", searchDescriptions);
            var pUseFullTextSearch = _dataProvider.GetBooleanParameter("UseFullTextSearch", _commonSettings.UseFullTextSearch);
            var pFullTextMode = _dataProvider.GetInt32Parameter("FullTextMode", (int)_commonSettings.FullTextMode);
            var pOrderBy = _dataProvider.GetInt32Parameter("OrderBy", (int)orderBy);
            var pPageIndex = _dataProvider.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = _dataProvider.GetInt32Parameter("PageSize", pageSize);
            var pShowHidden = _dataProvider.GetBooleanParameter("ShowHidden", showHidden);
            var pTotalRecords = _dataProvider.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var Service = _dbContext.EntityFromSql<Service>("ServiceLoadAllPaged",
                pCategoryIds,
                pStoreId,
                pKeywords,
                pSearchDescriptions,
                pUseFullTextSearch,
                pFullTextMode,
                pOrderBy,
                pPageIndex,
                pPageSize,
                pShowHidden,
                pTotalRecords).ToList();
            //return products
            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<Service>(Service, pageIndex, pageSize, totalRecords);
        }
        /// <summary>
        /// Inserts a Service item
        /// </summary>
        /// <param name="service">Service item</param>
        public virtual void InsertService(Service service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _serviceRepository.Insert(service);

            //event notification
            _eventPublisher.EntityInserted(service);
        }

        /// <summary>
        /// Updates the Service item
        /// </summary>
        /// <param name="service">Service item</param>
        public virtual void UpdateService(Service service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _serviceRepository.Update(service);

            //event notification
            _eventPublisher.EntityUpdated(service);
        }

        #endregion
        #endregion
    }
}