using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Entities;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    public class ReCaptchaLoginFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly ISiteService _siteService;
        private readonly SixLaborsCaptchaService _reCaptchaService;
        private readonly IShapeFactory _shapeFactory;

        public ReCaptchaLoginFilter(
            ILayoutAccessor layoutAccessor,
            ISiteService siteService,
            SixLaborsCaptchaService reCaptchaService,
            IShapeFactory shapeFactory)
        {
            _layoutAccessor = layoutAccessor;
            _siteService = siteService;
            _reCaptchaService = reCaptchaService;
            _shapeFactory = shapeFactory;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.Result is ViewResult || context.Result is PageResult)
                || !String.Equals("OrchardCore.Users", Convert.ToString(context.RouteData.Values["area"]), StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            //nothing to set
            var settings = (await _siteService.GetSiteSettingsAsync()).As<SixLaborsCaptchaSettings>();


            var layout = await _layoutAccessor.GetLayoutAsync();

            if (_reCaptchaService.IsThisARobot())
            {
                var afterLoginZone = layout.Zones["AfterLogin"];
                await afterLoginZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));
            }

            var afterForgotPasswordZone = layout.Zones["AfterForgotPassword"];
            await afterForgotPasswordZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            var afterRegisterZone = layout.Zones["AfterRegister"];
            await afterRegisterZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            var afterResetPasswordZone = layout.Zones["AfterResetPassword"];
            await afterResetPasswordZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            await next();
        }
    }
}