using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core.Features.Event;
using Reserve.Core.Features.MailService;
using Reserve.Core.Features.reCaptcha;
using Reserve.Helpers;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Event;

using Reserve.Core;

[BindProperties]
public class ReserveEventModel(IEventRepository eventRepository, IValidator<CasualTicketInput> validator, IEmailService emailService, ReCaptchaSettings reCaptchaSettings, IReCaptchaService reCaptchaService, IStringLocalizer<Global> localizer) : PageModel
{
    [BindProperty(SupportsGet = true)]
    [ValidateNever]
    public string? Id { get; set; }

    public CasualEventView? Event { get; set; }
    public CasualTicketInput Ticket { get; set; } = new();

    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;
    public string? ReCaptchaToken { get; set; }

    public async Task<IActionResult> OnGet()
    {
        CasualEvent? eventToReserve = await eventRepository.GetByIdAsync(GuidShortener.RestoreGuid(Id!).ToString());
        if (eventToReserve is null)
        {
            return RedirectToPage("EventError");
        }
        Event = new CasualEventView
        {
            Id = eventToReserve.Id,
            Title = eventToReserve.Title,
            OrganizerName = eventToReserve.OrganizerName,
            OrganizerEmail = eventToReserve.OrganizerEmail,
            Description = eventToReserve.Description,
            StartDate = eventToReserve.StartDate,
            EndDate = eventToReserve.EndDate,
            ImageUrl = eventToReserve.ImageUrl,
            Opened = eventToReserve.Opened,
            MaximumCapacity = eventToReserve.MaximumCapacity,
            CurrentCapacity = eventToReserve.CurrentCapacity,
        };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        Id = GuidShortener.RestoreGuid(Id!).ToString();
        CasualEvent? eventToReserve = await eventRepository.GetByIdAsync(Id);
        if (eventToReserve is null)
        {
            return RedirectToPage("EventError");
        }
        Event = new CasualEventView
        {
            Id = eventToReserve.Id,
            Title = eventToReserve.Title,
            OrganizerName = eventToReserve.OrganizerName,
            OrganizerEmail = eventToReserve.OrganizerEmail,
            Description = eventToReserve.Description,
            StartDate = eventToReserve.StartDate,
            EndDate = eventToReserve.EndDate,
            ImageUrl = eventToReserve.ImageUrl,
            Opened = eventToReserve.Opened,
            MaximumCapacity = eventToReserve.MaximumCapacity,
            CurrentCapacity = eventToReserve.CurrentCapacity,
        };
        Ticket.CasualEvent = eventToReserve;
        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(Ticket);
        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        if (!result.IsValid || !reCaptchaResult)
        {
            if (!reCaptchaResult)
            {
                ModelState.AddModelError("ReCaptchaToken", localizer["validation:recaptcha-required"]);
            }
            result.AddToModelState(this.ModelState, "Ticket");
        }
        else
        {
            CasualTicket ticket = new CasualTicket
            {
                ReserverName = Ticket.ReserverName,
                ReserverEmail = Ticket.ReserverEmail,
                CasualEvent = Ticket.CasualEvent,
                ReserverPhoneNumber = Ticket.ReserverPhoneNumber
            };
            var alreadyReserved = (await eventRepository.CheckIfAlreadyReserved(ticket)).ToList();
            if (alreadyReserved.Count != 0)
            {
                ModelState.AddModelError("Ticket.ReserverEmail", "This email is already reserved");
            }
            /*            ModelState.Remove("Id");*/
            ticket = (await eventRepository.AddReserverAsync(ticket))!;
            if (Ticket is not null)
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = Ticket.ReserverEmail,
                    Subject = "Event booked successfully!",
                    Body = ReservationSuccessfulNotificationEvent(GuidShortener.ShortenGuid(ticket.Id), baseUrl)
                };
                await emailService.SendEmailAsync(mailRequest);
                var ownerEmail = new MailRequest
                {
                    ToEmail = Event.OrganizerEmail,
                    Subject = "New event reservation Booked!",
                    Body = EventReservationNotificationOwner(GuidShortener.ShortenGuid(Event.Id).ToString(), baseUrl)
                };

                await emailService.SendEmailAsync(ownerEmail);
                return RedirectToPage("ReservationNotification", new { id = GuidShortener.ShortenGuid(ticket.Id) });
            }
            else
            {
                return RedirectToPage("EventError");
            }
        }
        return Page();
    }
}