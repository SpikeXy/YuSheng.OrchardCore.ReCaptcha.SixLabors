using System.ComponentModel.DataAnnotations;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels
{
    public class SixLaborsCaptchaSettingsViewModel
    {
        public int IpDetectionThreshold { get; set; }
        public int NumberOfCaptcha { get; set; } = 6;
        public bool SettingsAreConfigured { get; set; }
        public int TimeSpanMinuntes { get; set; } = 3;
        
        public byte DrawLines { get; set; } = 7;
        [MaxLength(2000)]
        public string LimitExceededWarning { get; set; }

    }
}
