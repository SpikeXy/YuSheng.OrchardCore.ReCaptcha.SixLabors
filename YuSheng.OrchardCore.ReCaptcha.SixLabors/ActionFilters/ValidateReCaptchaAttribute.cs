using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace OrchardCore.ReCaptcha.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateReCaptchaAttribute : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var recaptchaService = context.HttpContext.RequestServices.GetService<ReCaptchaService>();
            var S = context.HttpContext.RequestServices.GetService<IStringLocalizer<ReCaptchaService>>();
            var isValidCaptcha = false;
            var reCaptchaResponse = context.HttpContext.Request?.Form?[Constants.ReCaptchaServerResponseHeaderName].ToString();

            if (!String.IsNullOrWhiteSpace(reCaptchaResponse))
                isValidCaptcha = await recaptchaService.VerifyCaptchaResponseAsync(reCaptchaResponse);

            var isRobot = false;

            if (isRobot && !isValidCaptcha)
            {
                context.ModelState.AddModelError("ReCaptcha", S["Failed to validate captcha"]);
            }

            await next();

            if (context.ModelState.IsValid)
            {
                recaptchaService.ThisIsAHuman();
            }
            else
            {
                recaptchaService.MaybeThisIsARobot();
            }

            return;
        }
    }
}
