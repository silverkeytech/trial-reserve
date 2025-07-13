using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;

namespace Reserve.Pages.Appointment;

public class UserDetailsModel(IAppointmentRepository appointmentRepository, IEmailService emailService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public AppointmentDetails? AppointmentDetails { get; set; }
    public List<Availability?> FreeSlots { get; set; } = [];

    [BindProperty]
    public string? SelectedSlot { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }
        AppointmentDetails = await appointmentRepository.GetAppointmentDetailsByIdAsync(Id);
        if (AppointmentDetails is null)
        {
            return RedirectToPage("AppointmentError");
        }
        FreeSlots = await appointmentRepository.GetFreeSlotsOfCalendarByIdAsync(AppointmentDetails);
        if (HttpContext.Request.Cookies["error"] is not null)
        {
            TempData["error"] = HttpContext.Request.Cookies["error"];
            HttpContext.Response.Cookies.Delete("error");
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        try
        {
            AppointmentDetails = await appointmentRepository.GetAppointmentDetailsByIdAsync(Id!);
            if (AppointmentDetails is null)
            {
                return RedirectToPage("AppointmentError");
            }
            if (AppointmentDetails.AppointmentStatus == AppointmentState.Done)
            {
                TempData["error"] = "This appointment has already been completed.";
                return RedirectToPage("UserDetails", new { id = Id });
            }
            var result = await appointmentRepository.GetRescheduleByIdAsync(Id!);
            if (result is not null)
            {
                TempData["error"] = "You have already requested to reschedule this appointment.";
                return RedirectToPage("UserDetails", new { id = Id });
            }

            var requestedTime = await appointmentRepository.GetSlotByIdAsync(SelectedSlot!);
            if (requestedTime is null)
            {
                TempData["error"] = "The selected slot is not available. Please choose another slot.";
                return RedirectToPage("UserDetails", new { id = Id });
            }

            await appointmentRepository.CreateAppointmentReschedule(new AppointmentReschedule
            {
                OriginalAppointment = AppointmentDetails,
                RequestedTime = requestedTime,
                RescheduleStatus = RescheduleState.Pending
            });
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            MailRequest mailRequest = new MailRequest
            {
                ToEmail = AppointmentDetails.ReserverEmail,
                Subject = "Rescheduling Request",
                Body = ReschedulingRequestFromOwnerToClient(AppointmentDetails.Id.ToString(), baseUrl)
            };
            await emailService.SendEmailAsync(mailRequest);
            TempData["success"] = "Your request has been sent to the user.";
            return RedirectToPage("UpcomingAppointments", new { id = AppointmentDetails.Slot!.AppointmentCalendar?.Id });
        }
        catch
        {
            TempData["error"] = "Something went wrong. Please try again later.";
            return RedirectToPage("UpcomingAppointments", new { id = AppointmentDetails!.Slot!.AppointmentCalendar?.Id });
        }
    }
}