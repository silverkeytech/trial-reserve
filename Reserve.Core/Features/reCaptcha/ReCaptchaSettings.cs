using Microsoft.Extensions.Configuration;

namespace Reserve.Core.Features.reCaptcha
{
    public class ReCaptchaSettings
    {
        public string? SiteKey { get; private set; }
        public string? SecretKey { get; private set; }

        public ReCaptchaSettings(IConfiguration configuration)
        {
            var captchaConfig = configuration.GetSection("ReCaptchaSettings");
            SiteKey = captchaConfig["SiteKey"];
            SecretKey = captchaConfig["SecretKey"];
        }
    }
}