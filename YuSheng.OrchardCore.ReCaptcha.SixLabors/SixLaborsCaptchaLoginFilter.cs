using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

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

            var part = new SixLaborsCaptchaPart();
            //part.CaptchaImgBase64 = _sixLaborsCaptchaService.GetCaptchaPic(captchaString);
            var myShape = await _shapeFactory.CreateAsync("SixLaborsCaptchaPart", Arguments.From(new { SixLaborsCaptchaPart = part })); // 创建 MyContentPart 的 shape


            var layout = await _layoutAccessor.GetLayoutAsync();

            //var afterForgotPasswordZone = layout.Zones["AfterForgotPassword"];
            //await afterForgotPasswordZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));

            var afterRegisterZone = layout.Zones["AfterRegister"];
            await afterRegisterZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));

            //var afterResetPasswordZone = layout.Zones["AfterResetPassword"];
            //await afterResetPasswordZone.AddAsync(await _shapeFactory.CreateAsync("SixLaborsCaptchaPart"));

            await next();
        }
    }
}