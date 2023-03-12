using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    internal class Service
    {
        public Service() {

            var slc = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                DrawLines = 7,
                TextColor = new Color[] { Color.Blue, Color.Black },
            });

            var key = Extensions.GetUniqueKey(6);
            var result = slc.Generate(key);
            System.IO.File.WriteAllBytes($"six-labors-captcha.png", result);
        
        
        }
    }
}
