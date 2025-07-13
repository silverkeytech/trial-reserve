using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Event;
using Reserve.Helpers;

namespace Reserve.Pages.Event;

public class EventDetailsModel(IEventRepository eventRepository) : PageModel
{
    public CasualEventView? DetailedEvent { get; set; }

    public async Task<IActionResult> OnGet(string id)
    {
        CasualEvent? detailedEvent = await eventRepository.GetByIdAsync(GuidShortener.RestoreGuid(id).ToString());
        if (detailedEvent is null)
        {
            return RedirectToPage("EventError");
        }
        DetailedEvent = new CasualEventView
        {
            Id = detailedEvent.Id,
            Title = detailedEvent.Title,
            OrganizerName = detailedEvent.OrganizerName,
            OrganizerEmail = detailedEvent.OrganizerEmail,
            MaximumCapacity = detailedEvent.MaximumCapacity,
            Location = detailedEvent.Location,
            StartDate = detailedEvent.StartDate,
            EndDate = detailedEvent.EndDate,
            Opened = detailedEvent.Opened,
            Description = detailedEvent.Description,
            ImageUrl = detailedEvent.ImageUrl,
            CurrentCapacity = detailedEvent.CurrentCapacity
        };
        return Page();
    }
}