using System;
using System.Threading.Tasks;
using OrchardCore.Users.Events;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace OrchardCore.ReCaptcha.Users.Handlers
{
    public class PasswordRecoveryFormEventEventHandler : IPasswordRecoveryFormEvents
    {
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;

        public PasswordRecoveryFormEventEventHandler(SixLaborsCaptchaService sixLaborsReCaptchaService)
        {
            _sixLaborsCaptchaService = sixLaborsReCaptchaService;
        }

        public Task RecoveringPasswordAsync(Action<string, string> reportError)
        {
            return _sixLaborsCaptchaService.ValidateCaptchaAsync(reportError);
        }

        public Task PasswordResetAsync(PasswordRecoveryContext context)
        {
            return Task.CompletedTask;
        }

        public Task ResettingPasswordAsync(Action<string, string> reportError)
        {
            return _sixLaborsCaptchaService.ValidateCaptchaAsync(reportError);
        }

        public Task PasswordRecoveredAsync(PasswordRecoveryContext context)
        {
            return Task.CompletedTask;
        }
    }
}
