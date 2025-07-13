namespace Reserve.Core.Features.reCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        public async Task<bool> VerifyReCaptchaAsync(string reCaptchaResponse, ReCaptchaSettings reCaptchaSettings)
        {
            using var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSettings.SecretKey}&response={reCaptchaResponse}", null);
            var jsonString = await response.Content.ReadAsStringAsync();

            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
           
            if (jsonData != null && jsonData.TryGetValue("success", out var successValue))
            {
                if (successValue is bool success)
                {
                    return success;
                }
            }

            return false;
        }
    }
}
