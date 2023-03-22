using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    public class SixLaborsCaptchaLoginFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;
        private readonly SixLaborsCaptchaSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SixLaborsCaptchaLoginFilter(
            ILayoutAccessor layoutAccessor,
            IHttpContextAccessor httpContextAccessor,
            SixLaborsCaptchaService sixLaborsCaptchaService, 
            IOptions<SixLaborsCaptchaSettings> settingsAccessor)
        {
            _layoutAccessor = layoutAccessor;
            _settings = settingsAccessor.Value;
            _sixLaborsCaptchaService = sixLaborsCaptchaService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.Result is ViewResult || context.Result is PageResult)
                || !String.Equals("OrchardCore.Users", Convert.ToString(context.RouteData.Values["area"]), StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var captchaString = _sixLaborsCaptchaService.GetCaptchaString(_settings.NumberOfCaptcha);
            _httpContextAccessor.HttpContext.Session.SetString(Constants.SixLaborsCaptchaName, captchaString);
            var model = new SixLaborsCaptchaViewModel();
            model.SettingsAreConfigured = true;
            var imageBytes = _sixLaborsCaptchaService.GetCaptchaPic(captchaString, _settings.DrawLines, _settings.CaptchatHeight);
            string base64 = Convert.ToBase64String(imageBytes);
            model.CaptchaImgBase64 = $"data:image/jpeg;base64,{base64}";
            var shape = new ShapeViewModel<SixLaborsCaptchaViewModel>("SixLaborsCaptcha", model);

            var layout = await _layoutAccessor.GetLayoutAsync();

            var afterLoginZone = layout.Zones["AfterLogin"];
            await afterLoginZone.AddAsync(shape);

            var afterForgotPasswordZone = layout.Zones["AfterForgotPassword"];
            await afterForgotPasswordZone.AddAsync(shape);

            var afterRegisterZone = layout.Zones["AfterRegister"];
            //await afterRegisterZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));
            await afterRegisterZone.AddAsync(shape);


            var afterResetPasswordZone = layout.Zones["AfterResetPassword"];
            await afterResetPasswordZone.AddAsync(shape);

            await next();
        }
    }
}