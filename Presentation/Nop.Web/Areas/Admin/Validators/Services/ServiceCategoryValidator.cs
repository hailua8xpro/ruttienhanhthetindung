using FluentValidation;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Core.Domain.Services;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Services
{
    public class ServiceCategoryValidator: BaseNopValidator<ServiceCategoryModel>
    {
        public ServiceCategoryValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Service.Categories.Fields.Name.Required"));
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));
            SetDatabaseValidationRules<ServiceCategory>(dbContext);
        }
    }
}
