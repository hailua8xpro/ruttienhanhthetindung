using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Services
{
    public partial class ServiceCategoryMapping : BaseEntity
    {
        public int ServiceId { get; set; }
        public int CategoryId { get; set; }
        public virtual  ServiceCategory Category { get; set; }
        public virtual  Service Service { get; set; }
    }
}
