using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.MailService;
using Reserve.Core.Features.reCaptcha;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Appointment;

[BindProperties]
public class ReserverDetailsModel(IAppointmentRepository appointmentRepository, IValidator<AppointmentDetails?>? validator, IEmailService emailService, ReCaptchaSettings reCaptchaSettings, IReCaptchaService reCaptchaService, IStringLocalizer<Global> localizer) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public Availability? AvailabilitySlot { get; set; }
    public AppointmentDetails? Appointment { get; set; }

    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;

    public string? ReCaptchaToken { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (Id is null)
        {
            return RedirectToPage("AppointmentError");
        }

        AvailabilitySlot = await appointmentRepository.GetSlotByIdAsync(Id);
        if (AvailabilitySlot is null)
        {
            return RedirectToPage("AppointmentError");
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Id is null)
        {
            return RedirectToPage("AppointmentError");
        }

        AvailabilitySlot = await appointmentRepository.GetSlotByIdAsync(Id);
        if (AvailabilitySlot is null)
        {
            return RedirectToPage("AppointmentError");
        }

        if (Appointment is null)
        {
            return RedirectToPage("AppointmentError");
        }

        Appointment.Slot = AvailabilitySlot;
        Appointment.Slot.AppointmentCalendar = AvailabilitySlot.AppointmentCalendar;
        if (validator is null)
        {
            return RedirectToPage("AppointmentError");
        }
        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(Appointment);
        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        if (!result.IsValid || !reCaptchaResult)
        {
            if (!reCaptchaResult)
            {
                ModelState.AddModelError("ReCaptchaToken", localizer["validation:recaptcha-required"]);
            }
            result.AddToModelState(this.ModelState, "Appointment");
            return Page();
        }
        else
        {
            Appointment = await appointmentRepository.CreateAppointmentMeetingAsync(Appointment);
            if (Appointment is null)
            {
                return RedirectToPage("AppointmentError");
            }
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var bookingEmail = new MailRequest
            {
                ToEmail = Appointment.ReserverEmail,
                Subject = "Appointment Confirmation!",
                Body = AppointmentCreationNotificationClient(Appointment.Id.ToString(), baseUrl)
            };
            await emailService.SendEmailAsync(bookingEmail);

            var calendar = AvailabilitySlot.AppointmentCalendar;
            if (calendar is null)
            {
                return RedirectToPage("AppointmentError");
            }

            var ownerEmail = new MailRequest
            {
                ToEmail = calendar.Email,
                Subject = "New Appointment Booked!",
                Body = AppointmentCreationNotificationOwner(calendar.Id.ToString(), baseUrl)
            };

            await emailService.SendEmailAsync(ownerEmail);

            return RedirectToPage("MeetingNotification", new { id = Appointment.Id });
        }
    }
}