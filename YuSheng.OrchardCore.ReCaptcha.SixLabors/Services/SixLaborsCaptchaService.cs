using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public byte[] GetCaptchaPic(string key)
        {
            var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                DrawLines = 7,
                TextColor = new Color[] { Color.Blue, Color.Black },
            });
            var bytes = slc.Generate(key);
            return bytes;
        }

        public Task<bool> ValidateCaptchaAsync(Action<string, string> reportError)
        {
            var sessionCaptcha = _httpContextAccessor.HttpContext.Session.GetString(Constants.SixLaborsCaptchaName);
            var captchaRequest = _httpContextAccessor.HttpContext?.Request.Form[Constants.SixLaborsCaptchaName].ToString();

            var isValid = !String.IsNullOrEmpty(captchaRequest) && sessionCaptcha.Equals(captchaRequest, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                reportError("SixLaborsCaptcha", S["Failed to validate captcha"]);
            }

            return Task.FromResult(isValid);
        }
    }
}
