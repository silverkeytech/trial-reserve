using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Appointment;

public class RescheduleDetailsModel(IAppointmentRepository appointmentRepository, IEmailService emailService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public AppointmentReschedule? RescheduleDetails { get; set; }

    public AppointmentCalendar? AppointmentCalendar { get; set; }

    public async Task OnGet()
    {
        if (Id != null)
        {
            RescheduleDetails = await appointmentRepository.GetRescheduleByIdAsync(Id);
        }
    }

    public async Task<IActionResult> OnPostDecline()
    {
        if (Id != null)
        {
            RescheduleDetails = await appointmentRepository.GetRescheduleByIdAsync(Id);
            if (RescheduleDetails == null)
            {
                return RedirectToPage("AppointmentError");
            }
            await appointmentRepository.DeclineRescheduling(RescheduleDetails.Id.ToString());
            TempData["reschedule-request"] = "Reschedule request recorded successfully";
            return RedirectToPage("UserAppointmentDashboard", new { id = Id });
        }
        else
        {
            return RedirectToPage("AppointmentError");
        }
    }

    public async Task<IActionResult> OnPostAccept()
    {
        if (Id != null)
        {
            RescheduleDetails = await appointmentRepository.GetRescheduleByIdAsync(Id);
            if (RescheduleDetails == null || AppointmentCalendar == null)
            {
                return RedirectToPage("AppointmentError");
            }

            if (RescheduleDetails.RequestedTime?.AppointmentCalendar?.Email != null)
            {
                AppointmentCalendar = await appointmentRepository.GetCalendarFromEmail(RescheduleDetails.RequestedTime.AppointmentCalendar.Email);
            }
            else
            {
                return RedirectToPage("AppointmentError");
            }

            await appointmentRepository.AcceptRescheduling(RescheduleDetails.Id.ToString());
            TempData["reschedule-request"] = "Reschedule request recorded successfully";

            // Send email to the calendar owner
            if (AppointmentCalendar != null)
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = AppointmentCalendar.Email,
                    Subject = "Rescheduling Request Accepted",
                    Body = ReschedulingRequestAcceptedFromClietToOwner(AppointmentCalendar.Id.ToString(), baseUrl)
                };
                await emailService.SendEmailAsync(mailRequest);
            }

            return RedirectToPage("UserAppointmentDashboard", new { id = Id });
        }
        else
        {
            return RedirectToPage("AppointmentError");
        }
    }
}