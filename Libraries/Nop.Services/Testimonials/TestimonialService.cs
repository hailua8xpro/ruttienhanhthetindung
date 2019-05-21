using System;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using Nop.Core.Domain.Testimonial;
using System.Linq;
using System.Collections.Generic;

namespace Nop.Services.Testimonials
{
    public class TestimonialService : ITestimonialService
    {
        #region Fields
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Testimonial> _testimonialRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly string _entityName;
        #endregion
        #region Ctor
        public TestimonialService(CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IRepository<Testimonial> testimonialRepository,
            IRepository<StoreMapping> storeMappingRepository,
            ICacheManager cacheManager
            )
        {
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._testimonialRepository = testimonialRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._entityName = typeof(Testimonial).Name;
            this._cacheManager = cacheManager;
        }
        #endregion
        #region method

        #endregion
        public void DeleteTestimonial(Testimonial testimonial)
        {
            if (testimonial == null)
                throw new ArgumentNullException(nameof(Testimonial));

            _testimonialRepository.Delete(testimonial);

            //event notification
            _eventPublisher.EntityDeleted(testimonial);
        }

        public IPagedList<Testimonial> GetAllTestimonials(string keyword,int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _testimonialRepository.Table;
            if (!showHidden)
            {
                query = query.Where(b => b.Published);
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;

                query = query.Distinct();
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.FullName.Contains(keyword));
            }
            query = query.OrderBy(b => b.DisplayOrder);

            var Testimonials = new PagedList<Testimonial>(query, pageIndex, pageSize);
            return Testimonials;
        }

        public Testimonial GetTestimonialById(int testimonialId)
        {
            if (testimonialId == 0)
                return null;

            return _testimonialRepository.GetById(testimonialId);
        }

        public void InsertTestimonial(Testimonial Testimonial)
        {
            if (Testimonial == null)
                throw new ArgumentNullException(nameof(Testimonial));

            _testimonialRepository.Insert(Testimonial);
            _cacheManager.RemoveByPattern(NopTestimonialDefaults.TestimonialsPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(Testimonial);
        }

        public IList<Testimonial> PrepareHomeTestimonial(int storeId)
        {
            var query = _testimonialRepository.Table.Where(t=>!t.Deleted && t.Published);
            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;

                query = query.Distinct();
            }
            return query.ToList();
        }

        public void UpdateTestimonial(Testimonial Testimonial)
        {
            if (Testimonial == null)
                throw new ArgumentNullException(nameof(Testimonial));

            _testimonialRepository.Update(Testimonial);

            //event notification
            _eventPublisher.EntityUpdated(Testimonial);
        }
    }
}
