namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration
{
    public class SixLaborsCaptchaSettings
    {
        /// <summary>
        /// Maximum number of retries for a single IP address
        /// </summary>
        public int IpDetectionThreshold { get; set; } = 10;

        /// <summary>
        /// Number of captcha digits
        /// </summary>
        public int NumberOfCaptcha { get; set; } = 4;

        /// <summary>
        /// Lockout time after exceeding the limit
        /// </summary>
        public int TimeSpanMinuntes { get; set; } = 3;

        /// <summary>
        /// SixLabors Captcha Draw Lines
        /// </summary>
        public byte DrawLines { get; set; } = 3;

        public string LimitExceededWarning { get; set; } = "Your IP has exceeded the maximum number of requests allowed. Please try again later.";

        public bool IsValid()
        {
            return IpDetectionThreshold >= 1 && IpDetectionThreshold <= int.MaxValue && NumberOfCaptcha >= 1 && NumberOfCaptcha <= 20;
        }
    }
}
