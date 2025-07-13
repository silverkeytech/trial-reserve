using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;

namespace Reserve.Pages.Appointment;

[BindProperties]
public class UserAppointmentDashboardModel(IAppointmentRepository appointmentRepository) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public AppointmentDetails? AppointmentDetails { get; set; }
    public List<DateTime> FreeSlots { get; set; } = [];
    public DateTime SelectedDate { get; set; }
    public AppointmentReschedule? AppointmentReschedule { get; set; }

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

        if (HttpContext.Request.Cookies["check"] is not null)
        {
            TempData["check"] = HttpContext.Request.Cookies["check"];
            HttpContext.Response.Cookies.Delete("check");
        }

        List<Availability?>? availableSlots = await appointmentRepository.GetFreeSlotsOfCalendarByIdAsync(AppointmentDetails);

        FreeSlots = availableSlots.Where(x => x != null).Select(x => x!.StartTime).ToList();

        AppointmentReschedule = await appointmentRepository.GetRescheduleByIdAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }

        AppointmentDetails = await appointmentRepository.GetAppointmentDetailsByIdAsync(Id);
        if (AppointmentDetails?.AppointmentStatus == AppointmentState.Done)
        {
            TempData["error"] = "This appointment has already been completed.";
            return RedirectToPage("UserAppointmentDashboard", new { id = Id });
        }

        AppointmentReschedule? appointmentReschedule = await appointmentRepository.GetRescheduleByIdAsync(Id);
        if (appointmentReschedule is not null)
        {
            TempData["check"] = "Please check notifications before making this action";
            return RedirectToPage("UserAppointmentDashboard", new { id = Id });
        }

        if (AppointmentDetails is null)
        {
            return RedirectToPage("AppointmentError");
        }

        await appointmentRepository.Reschedule(AppointmentDetails, SelectedDate);
        return RedirectToPage("AppointmentRescheduleNotification", new { id = Id });
    }
}