using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Appointment;

namespace Reserve.Pages.Appointment;

public class FreeSlotModel(IAppointmentRepository appointmentRepository) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public Availability? FreeSlot { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("AppointmentError");
        }
        FreeSlot = await appointmentRepository.GetSlotByIdAsync(Id);
        if (FreeSlot is null)
        {
            return RedirectToPage("AppointmentError");
        }
        return Page();
    }
}