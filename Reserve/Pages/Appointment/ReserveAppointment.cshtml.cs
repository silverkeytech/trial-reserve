using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Reserve.Core.Features.Appointment;

namespace Reserve.Pages.Appointment;

public class ReserveAppointmentModel(IAppointmentRepository appointmentRepository) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public AppointmentCalendar? AppointmentCalendar { get; set; }

    public List<Availability?>? AvailabilitySlots { get; set; } = [];

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }
        AppointmentCalendar = await appointmentRepository.GetByIdAsync(Id);
        if (AppointmentCalendar is null)
        {
            return RedirectToPage("AppointmentError");
        }
        AvailabilitySlots = await appointmentRepository.GetOpenSlotsAsync(Id);
        return Page();
    }
}