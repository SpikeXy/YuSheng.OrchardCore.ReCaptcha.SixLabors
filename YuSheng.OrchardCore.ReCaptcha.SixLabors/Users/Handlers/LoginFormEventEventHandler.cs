using System;
using System.Threading.Tasks;
using OrchardCore.Users;
using OrchardCore.Users.Events;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace OrchardCore.ReCaptcha.Users.Handlers
{
    public class LoginFormEventEventHandler : ILoginFormEvent
    {
        private readonly SixLaborsCaptchaService _reCaptchaService;

        public LoginFormEventEventHandler(SixLaborsCaptchaService reCaptchaService)
        {
            _reCaptchaService = reCaptchaService;
        }

        public Task IsLockedOutAsync(IUser user) => Task.CompletedTask;

        public Task LoggedInAsync(IUser user)
        {
            _reCaptchaService.ThisIsAHuman();
            return Task.CompletedTask;
        }

        public async Task LoggingInAsync(string userName, Action<string, string> reportError)
        {
            if (_reCaptchaService.IsThisARobot())
            {
                await _reCaptchaService.ValidateCaptchaAsync(reportError);
            }
        }

        public Task LoggingInFailedAsync(string userName)
        {
            _reCaptchaService.MaybeThisIsARobot();
            return Task.CompletedTask;
        }

        public Task LoggingInFailedAsync(IUser user)
        {
            _reCaptchaService.MaybeThisIsARobot();
            return Task.CompletedTask;
        }
    }
}
