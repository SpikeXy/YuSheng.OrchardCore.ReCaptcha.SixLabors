using SixLabors.ImageSharp;
using SixLaborsCaptcha.Core;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors.Services
{
    public class SixLaborsCaptchaService
    {
        public SixLaborsCaptchaService()
        {



        }

        public byte[] GetCaptchaPic()
        {
            var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                DrawLines = 7,
                TextColor = new Color[] { Color.Blue, Color.Black },
            });

            var key = Extensions.GetUniqueKey(6);
            var bytes = slc.Generate(key);
            return bytes;
        }
    }
}
