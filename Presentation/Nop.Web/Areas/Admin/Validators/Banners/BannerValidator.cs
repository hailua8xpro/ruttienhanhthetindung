using FluentValidation;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Core.Domain.Banners;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Banners
{
    public class BannerValidator : BaseNopValidator<BannerModel>
    {
        public BannerValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Banners.Fields.Title.Required"));
            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Banners.Fields.Type.Required"));
            RuleFor(x => x.PictureId)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.ContentManagement.Banners.Fields.PictureId.Required"));
            RuleFor(x => x.PictureId)
            .NotEqual(0)
            .WithMessage(localizationService.GetResource("Admin.ContentManagement.Banners.Fields.PictureId.Required"));
            SetDatabaseValidationRules<Banner>(dbContext);
        }
    }
}
