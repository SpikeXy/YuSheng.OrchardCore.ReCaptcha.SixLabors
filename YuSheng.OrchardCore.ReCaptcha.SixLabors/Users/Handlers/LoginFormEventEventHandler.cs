using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrchardCore.Users;
using OrchardCore.Users.Events;
using YuSheng.OrchardCore.ReCaptcha.SixLabors;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace OrchardCore.ReCaptcha.Users.Handlers
{
    public class LoginFormEventEventHandler : ILoginFormEvent
    {
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginFormEventEventHandler(IHttpContextAccessor httpContextAccessor,
            SixLaborsCaptchaService sixLaborsReCaptchaService)
        {
            _sixLaborsCaptchaService = sixLaborsReCaptchaService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task IsLockedOutAsync(IUser user) => Task.CompletedTask;

        public Task LoggedInAsync(IUser user)
        {
            return Task.CompletedTask;
        }

        public Task LoggingInAsync(string userName, Action<string, string> reportError)
        {
            reportError("ReCaptcha", "Failed to validate captcha");
            return Task.FromResult(false);
            //return Task.CompletedTask;
        }

        public Task LoggingInFailedAsync(string userName)
        {
            return Task.CompletedTask;
        }

        public Task LoggingInFailedAsync(IUser user)
        {
            return Task.CompletedTask;
        }
    }
}
