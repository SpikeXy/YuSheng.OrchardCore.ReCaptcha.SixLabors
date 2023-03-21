using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLaborsCaptcha.Core;
using System;
using System.Linq;
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
        private readonly Color[] _colors = new Color[6] {
                                                        Color.Blue,
                                                        Color.Black,
                                                        Color.Black,
                                                        Color.Brown,
                                                        Color.Gray,
                                                        Color.Green
                                                        };
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


        public void RemoveValidateCount(string ipAddressKey)
        {
            _memoryCache.Remove(ipAddressKey);
        }

        public void CountValidateCaptcha(string ipAddressKey, TimeSpan timeSpan)
        {
            var faultyRequestCount = _memoryCache.GetOrCreate(ipAddressKey, fact => 0);
            faultyRequestCount++;
            _memoryCache.Set(ipAddressKey, faultyRequestCount, timeSpan);
        }

        private string GetIpAddress()
        {
            return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        private string GetIpAddressCacheKey()
        {
            return $"{IpAddressAbuseDetectorCacheKey}:{GetIpAddress()}";
        }


        public string GetCaptchaString(int number)
        {
            var key = Extensions.GetUniqueKey(number);
            return key;
        }
        public byte[] GetCaptchaPic(string key, byte drawLinesCount)
        {
            var random = new Random();
            var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                DrawLines = drawLinesCount,
                TextColor = _colors.ToList().OrderBy(x => random.Next()).Take(3).ToArray()
            });
            var bytes = slc.Generate(key);
            return bytes;
        }

        public Task<bool> ValidateCaptchaAsync(Action<string, string> reportError)
        {
            //Determine whether the verification code has exceeded the maximum number of attempts allowed.
            var cacheKey = GetIpAddressCacheKey();
            var timeSpanMinutes = _settings.TimeSpanMinuntes;
            var faultyRequestCount = _memoryCache.Get<int>(cacheKey);
            if (faultyRequestCount > _settings.IpDetectionThreshold)
            {
                reportError("SixLaborsCaptcha", S[_settings.LimitExceededWarning]);
                _logger.LogError($"ip address {GetIpAddress()} recaptcha error : The request verification code for the ip address exceeds the limit {_settings.IpDetectionThreshold}");
                return Task.FromResult(false);
            }
            else
            {
                CountValidateCaptcha(cacheKey, TimeSpan.FromMinutes(timeSpanMinutes));
                var sessionCaptcha = _httpContextAccessor.HttpContext.Session.GetString(Constants.SixLaborsCaptchaName);
                var captchaRequest = _httpContextAccessor.HttpContext?.Request.Form[Constants.SixLaborsCaptchaName].ToString();

                var isValid = !String.IsNullOrEmpty(captchaRequest) && sessionCaptcha != null && sessionCaptcha.Equals(captchaRequest, StringComparison.OrdinalIgnoreCase);

                if (!isValid)
                {
                    reportError("SixLaborsCaptcha", S["Failed to validate captcha"]);
                }
                else
                {
                    RemoveValidateCount(cacheKey);
                }

                return Task.FromResult(isValid);
            }


        }
    }
}
