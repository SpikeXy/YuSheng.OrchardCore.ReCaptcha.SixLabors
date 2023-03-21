using System.ComponentModel.DataAnnotations;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.ViewModels
{
    public class SixLaborsCaptchaViewModel
    {
        public bool SettingsAreConfigured { get; set; }
        public string CaptchaImgBase64 { get; set; }

        [Required(ErrorMessage = "Captcha is required.")]
        [Display(Name = "SixLaborsCaptcha")]
        public string SixLaborsCaptcha { get; set; }
    }
}
