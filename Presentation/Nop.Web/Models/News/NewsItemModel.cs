﻿using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using Nop.Web.Validators.News;

namespace Nop.Web.Models.News
{
    [Validator(typeof(NewsItemValidator))]
    public partial class NewsItemModel : BaseNopEntityModel
    {
        public NewsItemModel()
        {
            Comments = new List<NewsCommentModel>();
            AddNewComment = new AddNewsCommentModel();
        }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public PictureModel PictureModel { get; set; }
        public string Title { get; set; }
        public string Short { get; set; }
        public string Full { get; set; }
        public bool AllowComments { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string NewsCategoryName { get; set; }
        public string NewsCategorySeName { get; set; }
        public IList<NewsCommentModel> Comments { get; set; }
        public AddNewsCommentModel AddNewComment { get; set; }
        public IList<NewsCategoryModel> CategoryBreadcrumb { get; set; }
    }
}