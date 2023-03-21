using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    public class SixLaborsCaptchaLoginFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly ISiteService _siteService;
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;
        private readonly IShapeFactory _shapeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SixLaborsCaptchaLoginFilter(
            ILayoutAccessor layoutAccessor,
            IHttpContextAccessor httpContextAccessor,
            ISiteService siteService,
            SixLaborsCaptchaService sixLaborsCaptchaService,
            IShapeFactory shapeFactory)
        {
            _layoutAccessor = layoutAccessor;
            _siteService = siteService;
            _sixLaborsCaptchaService = sixLaborsCaptchaService;
            _shapeFactory = shapeFactory;
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
            var captchaString = _sixLaborsCaptchaService.GetCaptchaString();
            _httpContextAccessor.HttpContext.Session.SetString(Constants.SixLaborsCaptchaName, captchaString);
            var model = new SixLaborsCaptchaViewModel();
            model.SettingsAreConfigured = true;
            var imageBytes = _sixLaborsCaptchaService.GetCaptchaPic(captchaString);
            string base64 = Convert.ToBase64String(imageBytes);
            model.CaptchaImgBase64 = $"data:image/jpeg;base64,{base64}";

            var layout = await _layoutAccessor.GetLayoutAsync();
            var afterLoginZone = layout.Zones["AfterLogin"];
            await afterLoginZone.AddAsync(new ShapeViewModel<SixLaborsCaptchaViewModel>("SixLaborsCaptcha", model));



            //var afterForgotPasswordZone = layout.Zones["AfterForgotPassword"];
            //await afterForgotPasswordZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));

            //var afterRegisterZone = layout.Zones["AfterRegister"];
            ////await afterRegisterZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));
            //await afterRegisterZone.AddAsync(part);


            //var afterResetPasswordZone = layout.Zones["AfterResetPassword"];
            //await afterResetPasswordZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));

            await next();
        }
    }
}