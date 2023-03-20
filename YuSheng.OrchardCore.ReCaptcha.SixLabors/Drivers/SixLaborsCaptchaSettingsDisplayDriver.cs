using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers
{
    public class SixLaborsCaptchaSettingsDisplayDriver : SectionDisplayDriver<ISite, SixLaborsCaptchaSettings>
    {
        public const string GroupId = "sixLaborsCaptcha";
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public SixLaborsCaptchaSettingsDisplayDriver(
            IShellHost shellHost,
            ShellSettings shellSettings,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _shellHost = shellHost;
            _shellSettings = shellSettings;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public override async Task<IDisplayResult> EditAsync(SixLaborsCaptchaSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.SixLaborsCaptcha))
            {
                return null;
            }

            return Initialize<SixLaborsCaptchaSettingsViewModel>("SixLaborsCaptchaSettings_Edit", model =>
            {
                model.IpDetectionThreshold = settings.IpDetectionThreshold;
                model.NumberOfCaptcha = settings.NumberOfCaptcha;
            })
                .Location("Content")
                .OnGroup(GroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(SixLaborsCaptchaSettings section, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.SixLaborsCaptcha))
            {
                return null;
            }

            if (context.GroupId == GroupId)
            {
                var model = new SixLaborsCaptchaSettingsViewModel();

                if (await context.Updater.TryUpdateModelAsync(model, Prefix))
                {
                    section.IpDetectionThreshold = model.IpDetectionThreshold;
                    section.NumberOfCaptcha = model.NumberOfCaptcha;
                    // Release the tenant to apply settings.
                    await _shellHost.ReleaseShellContextAsync(_shellSettings);
                }
            }

            return await EditAsync(section, context);
        }
    }
}