using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;

namespace Reserve.Pages.Appointment;

public class UpcomingAppointmentsModel() : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public List<AppointmentDetails?>? Appointments { get; set; } = [];
    public Availability? NewAvailabilitySlot { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }
        if (HttpContext.Request.Cookies["error"] is not null)
        {
            TempData["error"] = HttpContext.Request.Cookies["error"];
            HttpContext.Response.Cookies.Delete("error");
        }
        if (HttpContext.Request.Cookies["success"] is not null)
        {
            TempData["success"] = HttpContext.Request.Cookies["success"];
            HttpContext.Response.Cookies.Delete("success");
        }
        return Page();
    }
}