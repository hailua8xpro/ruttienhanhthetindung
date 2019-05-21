using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    /// <summary>
    /// Category (for caching)
    /// </summary>
    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class CategoryForCaching : NewsCategory, IEntityForCaching
    {
        public CategoryForCaching()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="c">Category to copy</param>
        public CategoryForCaching(NewsCategory c)
        {
            Id = c.Id;
            Name = c.Name;
            Description = c.Description;
            MetaKeywords = c.MetaKeywords;
            MetaDescription = c.MetaDescription;
            MetaTitle = c.MetaTitle;
            ParentCategoryId = c.ParentCategoryId;
            PictureId = c.PictureId;
            IncludeInTopMenu = c.IncludeInTopMenu;
            SubjectToAcl = c.SubjectToAcl;
            LimitedToStores = c.LimitedToStores;
            Published = c.Published;
            Deleted = c.Deleted;
            DisplayOrder = c.DisplayOrder;
            CreatedOnUtc = c.CreatedOnUtc;
            UpdatedOnUtc = c.UpdatedOnUtc;
        }
    }
}