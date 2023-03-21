using Microsoft.Extensions.Options;
using OrchardCore.Entities;
using OrchardCore.Settings;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration
{
    public class SixLaborsCaptchaSettingsConfiguration : IConfigureOptions<SixLaborsCaptchaSettings>
    {
        private readonly ISiteService _site;

        public SixLaborsCaptchaSettingsConfiguration(ISiteService site)
        {
            _site = site;
        }

        public void Configure(SixLaborsCaptchaSettings options)
        {
            var settings = _site.GetSiteSettingsAsync()
                .GetAwaiter().GetResult()
                .As<SixLaborsCaptchaSettings>();

            options.IpDetectionThreshold = settings.IpDetectionThreshold;
            options.NumberOfCaptcha = settings.NumberOfCaptcha;
            options.TimeSpanMinuntes = settings.TimeSpanMinuntes;
            options.DrawLines = settings.DrawLines;
            options.LimitExceededWarning = settings.LimitExceededWarning;
        }
    }
}
