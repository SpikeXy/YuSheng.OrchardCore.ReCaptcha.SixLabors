using OrchardCore.Users;
using OrchardCore.Users.Events;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace OrchardCore.ReCaptcha.Users.Handlers
{
    public class RegistrationFormEventHandler : IRegistrationFormEvents
    {
        private readonly SixLaborsCaptchaService _sixLaborsCaptchaService;

        public RegistrationFormEventHandler(SixLaborsCaptchaService sixLaborsReCaptchaService)
        {
            _sixLaborsCaptchaService = sixLaborsReCaptchaService;
        }

        public Task RegisteredAsync(IUser user)
        {
            return Task.CompletedTask;
        }

        public Task RegistrationValidationAsync(Action<string, string> reportError)
        {
            return _sixLaborsCaptchaService.ValidateCaptchaAsync(reportError);
        }
    }
}
