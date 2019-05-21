using FluentValidation;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.News
{
    public class NewsCategoryValidator: BaseNopValidator<NewsCategoryModel>
    {
        public NewsCategoryValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));
            SetDatabaseValidationRules<NewsCategory>(dbContext);
        }
    }
}
