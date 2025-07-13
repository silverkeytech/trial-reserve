using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.reCaptcha;

namespace Reserve.Pages.Appointment;

[BindProperties]
public class CreateAppointmentCalendarModel(
    IAppointmentRepository appointmentRepository,
    IValidator<AppointmentCalendar> validator,
    IReCaptchaService reCaptchaService,
    ReCaptchaSettings reCaptchaSettings,
    IStringLocalizer<Global> localizer) : PageModel
{
    public AppointmentCalendar? NewAppointmentCalendar { get; set; }
    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;
    public string? ReCaptchaToken { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        var result = await validator.ValidateAsync(NewAppointmentCalendar!);
        if (result.IsValid)
        {
            AppointmentCalendar? calendar = await appointmentRepository.GetCalendarFromEmail(NewAppointmentCalendar?.Email!);
            if (calendar is not null)
            {
                ModelState.AddModelError("NewAppointmentCalendar.Email", localizer["validation:email-already-in-use"]);
                return Page();
            }

            NewAppointmentCalendar = await appointmentRepository.CreateAppointmentInfoAsync(NewAppointmentCalendar);
            if (NewAppointmentCalendar is null)
            {
                return RedirectToPage("AppointmentError");
            }

            return RedirectToPage("CreateCalendar", new { id = NewAppointmentCalendar.Id });
        }
        if (!reCaptchaResult)
        {
            ModelState.AddModelError("ReCaptchaToken", localizer["validation:recaptcha-required"]);
        }
        result.AddToModelState(ModelState, "NewAppointmentCalendar");
        return Page();
    }
}