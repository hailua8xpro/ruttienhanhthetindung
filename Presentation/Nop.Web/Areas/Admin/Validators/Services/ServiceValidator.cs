using FluentValidation;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Core.Domain.Services;

namespace Nop.Web.Areas.Admin.Validators.Services
{
    public partial class ServiceValidator : BaseNopValidator<ServiceModel>
    {
        public ServiceValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Service.Item.Fields.Name.Required"));

            RuleFor(x => x.Short).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Service.Item.Fields.Short.Required"));

            RuleFor(x => x.Full).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Service.Item.Fields.Full.Required"));

            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Service>(dbContext);
        }
    }
}