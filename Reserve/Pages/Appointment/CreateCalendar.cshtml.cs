using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Appointment;

[BindProperties]
public class CreateCalendarModel(IAppointmentRepository appointmentRepository, IEmailService emailService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public AppointmentCalendar? NewAppointmentCalendar { get; set; }
    public Availability? NewAvailabilitySlot { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }
        NewAppointmentCalendar = await appointmentRepository.GetByIdAsync(Id);
        if (NewAppointmentCalendar is null)
        {
            return RedirectToPage("AppointmentError");
        }
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        MailRequest mailRequest = new MailRequest
        {
            ToEmail = NewAppointmentCalendar.Email,
            Subject = "Calendar Created",
            Body = AppointmentCreationNotification(NewAppointmentCalendar.Id.ToString(), baseUrl)
        };
        await emailService.SendEmailAsync(mailRequest);
        return Page();
    }
}