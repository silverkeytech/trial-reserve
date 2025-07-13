using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Event;

namespace Reserve.Pages.Event;

public class FindEventModel(IEventRepository eventRepository) : PageModel
{
    public List<CasualEventView?> CasualEvents = new();

    public async Task<IActionResult> OnGet()
    {
        List<CasualEvent?> allEvents = await eventRepository.GetAllEvents();
        foreach (var casualEvent in allEvents)
        {
            if (casualEvent is null)
            {
                return RedirectToPage("/Event/EventError");
            }
            CasualEvents.Add(new CasualEventView
            {
                Id = casualEvent.Id,
                Title = casualEvent.Title,
                OrganizerName = casualEvent.OrganizerName,
                OrganizerEmail = casualEvent.OrganizerEmail,
                Description = casualEvent.Description,
                StartDate = casualEvent.StartDate,
                EndDate = casualEvent.EndDate,
                ImageUrl = casualEvent.ImageUrl,
                Opened = casualEvent.Opened,
                MaximumCapacity = casualEvent.MaximumCapacity,
                CurrentCapacity = casualEvent.CurrentCapacity,
            });
        }
        return Page();
    }
}