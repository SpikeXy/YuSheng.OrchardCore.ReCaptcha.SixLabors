using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Entities;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers
{
    public class SixLaborsCaptchaPartDisplayDriver : ContentPartDisplayDriver<SixLaborsCaptchaPart>
    {
        public const string GroupId = "sixLaborsCaptcha";
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;
        private readonly ISiteService _siteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;

        public SixLaborsCaptchaPartDisplayDriver(
            IShellHost shellHost,
            ISiteService siteService,
            ShellSettings shellSettings,
            SixLaborsCaptchaService sixLaborsCaptchaService,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _shellHost = shellHost;
            _siteService = siteService;
            _shellSettings = shellSettings;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public override IDisplayResult Display(SixLaborsCaptchaPart part, BuildPartDisplayContext context)
        {
            return Initialize("SixLaborsCaptchaPart", (Func<SixLaborsCaptchaPartViewModel, ValueTask>)(async m =>
            {
                var sixLaborsCaptchaString = _httpContextAccessor.HttpContext.Session.GetString(Constants.SixLaborsCaptchaName);
                var siteSettings = await _siteService.GetSiteSettingsAsync();
                var settings = siteSettings.As<SixLaborsCaptchaSettings>();
                var imageBytes = _sixLaborsCaptchaService.GetCaptchaPic(sixLaborsCaptchaString);
                string base64 = Convert.ToBase64String(imageBytes);
                m.CaptchaImgBase64 = $"data:image/jpeg;base64,{base64}";
                m.SettingsAreConfigured = settings.IsValid();
            })).Location("Detail", "Content");
        }

        public override IDisplayResult Edit(SixLaborsCaptchaPart part, BuildPartEditorContext context)
        {
            return Initialize<SixLaborsCaptchaPartViewModel>("SixLaborsCaptchaPart_Fields_Edit", async m =>
            {
                var siteSettings = await _siteService.GetSiteSettingsAsync();
                var settings = siteSettings.As<SixLaborsCaptchaSettings>();
                m.SettingsAreConfigured = settings.IsValid();
            });
        }
    }
}