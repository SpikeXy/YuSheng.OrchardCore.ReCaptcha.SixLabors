using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using SixLaborsCaptcha.Core;
using System;
using System.Threading.Tasks;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Services
{
    public class SixLaborsCaptchaService
    {
        private const string IpAddressAbuseDetectorCacheKey = "SixLaborsIpAddressRobotDetector";

        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SixLaborsCaptchaSettings _settings;
        private readonly IStringLocalizer S;

        public SixLaborsCaptchaService(IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SixLaborsCaptchaService> stringLocalizer,
            IMemoryCache memoryCache, IOptions<SixLaborsCaptchaSettings> settingsAccessor, ILogger<SixLaborsCaptchaService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _settings = settingsAccessor.Value;
            _logger = logger;
            S = stringLocalizer;
        }

        public void IsNotARobot()
        {
            var ipAddressKey = GetIpAddressCacheKey();
            _memoryCache.Remove(ipAddressKey);
        }

        public void ValidateCaptcha()
        {
            var ipAddressKey = GetIpAddressCacheKey();

            var faultyRequestCount = _memoryCache.GetOrCreate(ipAddressKey, fact => 0);
            faultyRequestCount++;
            _memoryCache.Set(ipAddressKey, faultyRequestCount);
        }

        private string GetIpAddress()
        {
            return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        private string GetIpAddressCacheKey()
        {
            return $"{IpAddressAbuseDetectorCacheKey}:{GetIpAddress()}";
        }


        public string GetCaptchaString()
        {
            var key = Extensions.GetUniqueKey(6);
            return key;
        }
        public byte[] GetCaptchaPic()
        {
            var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                DrawLines = 7,
                TextColor = new Color[] { Color.Blue, Color.Black },
            });
            var key = GetCaptchaString();
            var bytes = slc.Generate(key);
            return bytes;
        }

        public async Task<bool> ValidateCaptchaAsync(Action<string, string> reportError)
        {
            //if (!_settings.IsValid())
            //{
            //    _logger.LogWarning("The ReCaptcha settings are not valid");
            //    return false;
            //}

            var sessionStr = _httpContextAccessor.HttpContext.Session.GetString(Constants.SixLaborsCaptchaHeaderName);

            var captchaResponse = _httpContextAccessor.HttpContext?.Request.Headers[Constants.SixLaborsCaptchaHeaderName];

            // If this is a standard form post we get the token from the form values if not affected previously in the header
            if (String.IsNullOrEmpty(captchaResponse) && (_httpContextAccessor.HttpContext?.Request.HasFormContentType ?? false))
            {
                captchaResponse = _httpContextAccessor.HttpContext.Request.Form[Constants.SixLaborsCaptchaHeaderName].ToString();
            }

            var isValid = !String.IsNullOrEmpty(captchaResponse) && await VerifyCaptchaResponseAsync(reCaptchaResponse);

            if (!isValid)
            {
                reportError("ReCaptcha", S["Failed to validate captcha"]);
            }

            return isValid;
        }
    }
}
