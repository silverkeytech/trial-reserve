using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core;
using Reserve.Core.Features.MailService;
using Reserve.Core.Features.Queue;
using Reserve.Core.Features.reCaptcha;
using Reserve.Helpers;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Queue;

[BindProperties]
public class CreateQueueModel(IQueueRepository queueRepository, IValidator<QueueEvent> validator, IEmailService emailService, ReCaptchaSettings reCaptchaSettings, IReCaptchaService reCaptchaService, IStringLocalizer<Global> L) : PageModel
{
    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;

    public string? ReCaptchaToken { get; set; }

    public QueueEventInput? NewQueue { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (NewQueue == null)
        {
            ModelState.AddModelError(string.Empty, "NewQueue cannot be null.");
            return Page();
        }
        var newQueueEvent = new QueueEvent
        {
            Title = NewQueue.Title ?? string.Empty,
            OrganizerEmail = NewQueue.OrganizerEmail ?? string.Empty,
            Description = NewQueue.Description ?? string.Empty,
            CurrentNumberServed = 1,
            TicketCounter = 0,
            LastReset = DateTime.UtcNow
        };

        var validationResult = await validator.ValidateAsync(newQueueEvent);
        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        if (!validationResult.IsValid || !reCaptchaResult)
        {
            if (!reCaptchaResult)
            {
                ModelState.AddModelError("ReCaptchaToken", L["validation:recaptcha-required"]);
            }
            validationResult.AddToModelState(ModelState, "NewQueue");
            return Page();
        }

        newQueueEvent = await queueRepository.Create(newQueueEvent);
        if (newQueueEvent is null)
        {
            return RedirectToPage("QueueError");
        }

        string sendtopage = GuidShortener.ShortenGuid(newQueueEvent.Id);
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        MailRequest mailRequest = new MailRequest
        {
            ToEmail = NewQueue.OrganizerEmail,
            Subject = "Queue Created",
            Body = QueueCreationNotification(sendtopage, baseUrl)
        };
        await emailService.SendEmailAsync(mailRequest);

        return RedirectToPage("QueueURL", new { id = sendtopage });
    }
}