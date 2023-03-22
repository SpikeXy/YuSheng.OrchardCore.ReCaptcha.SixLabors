using System.ComponentModel.DataAnnotations;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels
{
    public class SixLaborsCaptchaSettingsViewModel
    {
        public int IpDetectionThreshold { get; set; }
        public int NumberOfCaptcha { get; set; } 
        public bool SettingsAreConfigured { get; set; }
        public int TimeSpanMinuntes { get; set; }
        
        public byte DrawLines { get; set; }
        [Range(5,int.MaxValue)]
        public ushort CaptchatHeight { get; set; }
        [MaxLength(2000)]
        public string LimitExceededWarning { get; set; }

    }
}
