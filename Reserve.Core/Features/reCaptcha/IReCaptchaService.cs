namespace Reserve.Core.Features.reCaptcha
{
    public interface IReCaptchaService
    {
        public Task<bool> VerifyReCaptchaAsync(string reCaptchaResponse, ReCaptchaSettings reCaptchaSettings);
    }
}