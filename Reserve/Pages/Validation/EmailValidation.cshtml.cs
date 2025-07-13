using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Validation
{
    public class EmailValidationModel : PageModel
    {
        private readonly IEmailService _emailService;

        public EmailValidationModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [BindProperty]
        public string? Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false });
            }

            var mailRequest = new MailRequest
            {
                ToEmail = Email,
                Subject = "Validation email from Reserve",
                Body = EmailServiceValidation()
            };

            try
            {
                await _emailService.SendEmailAsync(mailRequest);
                return new JsonResult(new { success = true });
            }
            catch
            {
                return new JsonResult(new { success = false });
            }
        }
    }
}