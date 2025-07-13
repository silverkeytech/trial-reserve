using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Queue;
using Reserve.Helpers;

namespace Reserve.Pages.Queue;

using Microsoft.Extensions.Localization;
using Reserve.Core;
using Reserve.Core.Features.NotificationService;
using Reserve.Core.Features.reCaptcha;

[BindProperties]
public class QueueRegistrationModel(IQueueRepository queueRepository, IValidator<QueueTicket> validator, ReCaptchaSettings reCaptchaSettings, IReCaptchaService reCaptchaService, IStringLocalizer<Global> L, NotificationSettings notificationSettings, IStringLocalizer<QueueLocalization> qL) : PageModel
{
    [BindProperty]
    public string? FcmToken { get; set; }

    [BindProperty]
    public QueueTicket? NewQueueTicket { get; set; } = new QueueTicket();

    public Guid QueueEventId { get; set; }
    public QueueEventView? QueueEvent { get; set; } = new QueueEventView();
    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;
    public int NextQueueNumber { get; set; }

    public string? ReCaptchaToken { get; set; }

    public async Task OnGet(string id)
    {
        QueueEventId = GuidShortener.RestoreGuid(id);
        QueueEvent? CurrentQueue = await queueRepository.GetQueueEventByID(QueueEventId.ToString());
        QueueEvent = new QueueEventView()
        {
            Id = CurrentQueue!.Id,
            Title = CurrentQueue.Title,
            OrganizerEmail = CurrentQueue.OrganizerEmail,
            Description = CurrentQueue.Description,
            CurrentNumberServed = CurrentQueue.CurrentNumberServed,
            TicketCounter = CurrentQueue.TicketCounter,
            LastReset = CurrentQueue.LastReset
        };
        NextQueueNumber = await queueRepository.GetNextQueueNumber(QueueEventId.ToString());
    }

    public async Task<IActionResult> OnPost()
    {
		var request = HttpContext.Request;
		NewQueueTicket!.QueueEvent = await queueRepository.GetQueueEventByID(QueueEventId.ToString());
        var validationResult = await validator.ValidateAsync(NewQueueTicket);

        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        if (!validationResult.IsValid || !reCaptchaResult)
        {
            if (!reCaptchaResult)
            {
                ModelState.AddModelError("ReCaptchaToken", L["validation:recaptcha-required"]);
            }
            validationResult.AddToModelState(ModelState, "NewQueueTicket");
            return Page();
        }

        if (NewQueueTicket.CustomerPhoneNumber == null)
        {
            ModelState.AddModelError("NewQueueTicket.CustomerPhoneNumber", "Phone number is required.");
            return Page();
        }

        bool doesPhoneNumberExist = await queueRepository.DoesPhoneNumberExist(NewQueueTicket.CustomerPhoneNumber, QueueEventId.ToString());
        if (doesPhoneNumberExist)
        {
            ModelState.AddModelError("NewQueueTicket.CustomerPhoneNumber", L["phone-number-already-exists"]);

            // Repopulate the data
            QueueEvent? CurrentQueue = await queueRepository.GetQueueEventByID(QueueEventId.ToString());
            QueueEvent = new QueueEventView()
            {
                Id = CurrentQueue!.Id,
                Title = CurrentQueue.Title,
                OrganizerEmail = CurrentQueue.OrganizerEmail,
                Description = CurrentQueue.Description,
                CurrentNumberServed = CurrentQueue.CurrentNumberServed,
                TicketCounter = CurrentQueue.TicketCounter,
                LastReset = CurrentQueue.LastReset
            };
            NextQueueNumber = await queueRepository.GetNextQueueNumber(QueueEventId.ToString());

            return Page();
        }

        NewQueueTicket.QueueNumber = await queueRepository.GetNextQueueNumber(QueueEventId.ToString());
        await queueRepository.IncrementTicketCounter(QueueEventId.ToString());
        NewQueueTicket = await queueRepository.RegisterCustomer(NewQueueTicket);
		string baseUrl = $"{request.Scheme}://{request.Host}";
			var url = $"{baseUrl}/q-ticket/" + GuidShortener.ShortenGuid(NewQueueTicket!.Id) + "/" + GuidShortener.ShortenGuid(QueueEventId);
        var title = qL["queue-registration-successful"];
        var body = qL["view-details-link"] + url;
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
             { "click_action", "WEB_NOTIFICATION_CLICK" },
             { "tag", "notification-tag" },
             { "url", url }
        };
        if (FcmToken != null)
        {
            string response = await notificationSettings.SendNotificationAsync(FcmToken, title, body, data);
        }
        return RedirectToPage("QueueTicket", new { QueueTicketId = GuidShortener.ShortenGuid(NewQueueTicket!.Id), QueueId = GuidShortener.ShortenGuid(QueueEventId) });
    }
}