using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.News
{
    public partial class NewsCategoryMapping : BaseEntity
    {
        public int NewsId { get; set; }
        public int CategoryId { get; set; }
        public virtual  NewsCategory Category { get; set; }
        public virtual  NewsItem NewsItem { get; set; }
    }
}
