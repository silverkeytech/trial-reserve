using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reserve.Core.Features.Event;
using Reserve.Helpers;

namespace Reserve.Pages.Event;

public class ViewAttendeesModel(IEventRepository eventRepository) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public List<CasualTicketView?> Attendees { get; set; } = [];
    public CasualEventView? Event { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (Id == null)
        {
            return RedirectToPage("EventError");
        }
        string restoredId = GuidShortener.RestoreGuid(Id).ToString();
        CasualEvent? eventDetails = await eventRepository.GetByIdAsync(restoredId);
        List<CasualTicket?> attendees = await eventRepository.GetAttendeesAsync(restoredId);
        if (eventDetails is null)
        {
            return RedirectToPage("EventError");
        }
        Event = new CasualEventView
        {
            Id = eventDetails.Id,
            Title = eventDetails.Title,
            OrganizerName = eventDetails.OrganizerName,
            OrganizerEmail = eventDetails.OrganizerEmail,
            Location = eventDetails.Location,
            Description = eventDetails.Description,
            StartDate = eventDetails.StartDate,
            EndDate = eventDetails.EndDate,
            ImageUrl = eventDetails.ImageUrl,
            Opened = eventDetails.Opened,
            MaximumCapacity = eventDetails.MaximumCapacity,
            CurrentCapacity = eventDetails.CurrentCapacity,
        };
        foreach (var attendee in attendees)
        {
            Attendees.Add(new CasualTicketView
            {
                Id = attendee!.Id,
                ReserverName = attendee.ReserverName,
                ReserverEmail = attendee.ReserverEmail,
                ReserverPhoneNumber = attendee.ReserverPhoneNumber,
                CasualEvent = attendee.CasualEvent,
            });
        }
        return Page();
    }
}