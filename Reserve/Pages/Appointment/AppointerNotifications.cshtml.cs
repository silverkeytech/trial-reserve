using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;

namespace Reserve.Pages.Appointment;

public class AppointerNotificationsModel(IAppointmentRepository appointmentRepository) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public List<AppointerNotifications?> AppointerNotifications { get; set; } = [];

    public List<AppointmentReschedule?> RescheduleRequests { get; set; } = [];

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }

        AppointerNotifications = (await appointmentRepository.GetAppointmentNotificationsForCalendarAsync(Id)) ?? [];

        if (AppointerNotifications.Count == 0)
        {
            return RedirectToPage("AppointmentError");
        }

        RescheduleRequests = await appointmentRepository.GetAllRequestsForCalendar(Id);
        return Page();
    }
}