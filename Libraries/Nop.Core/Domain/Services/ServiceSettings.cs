using Nop.Core.Configuration;

namespace Nop.Core.Domain.Services
{
    /// <summary>
    /// Service settings
    /// </summary>
    public class ServiceSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Service are enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show Service on the main page
        /// </summary>
        public bool ShowServiceOnMainPage { get; set; }
        public int ServicePageSize { get; set; }
        public bool ShowServicesFromSubcategories { get; set; }
        public bool ServiceCategoryBreadcrumbEnabled { get; set; }
    }
}