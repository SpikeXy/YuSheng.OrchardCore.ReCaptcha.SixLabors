namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration
{
    public class SixLaborsCaptchaSettings
    {
        /// <summary>
        /// The maximum detection value of an IP address.
        /// </summary>
        public int IpDetectionThreshold { get; set; } = 5;

        public int NumberOfCaptcha { get; set; } = 6;

        public bool IsValid()
        {
            return IpDetectionThreshold>=1 && IpDetectionThreshold <= int.MaxValue && NumberOfCaptcha >= 1 && NumberOfCaptcha <= 20;
        }
    }
}
