using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core.Features.Queue;
using Reserve.Helpers;
using System.Text;

namespace Reserve.Pages.Queue;

public class QueueAdminModel(IQueueRepository queueRepository, IValidator<QueueTicket?> validator, IStringLocalizer<QueueLocalization> L) : PageModel
{
    [BindProperty]
    public Guid Id { get; set; }

    public List<QueueTicket?> Attendees { get; set; } = [];
    public DateTime LastReset { get; set; }

    public async Task<IActionResult> OnGet(string id)
    {
        Id = GuidShortener.RestoreGuid(id);
        Attendees = await queueRepository.GetAttendees(Id.ToString());
        QueueEvent? queueEvent = await queueRepository.GetQueueEventByID(Id.ToString());
        QueueEventView queueEventView = new QueueEventView()
        {
            Id = queueEvent!.Id,
            Title = queueEvent.Title,
            OrganizerEmail = queueEvent.OrganizerEmail,
            Description = queueEvent.Description,
            CurrentNumberServed = queueEvent.CurrentNumberServed,
            TicketCounter = queueEvent.TicketCounter,
            LastReset = queueEvent.LastReset
        };
        LastReset = queueEventView.LastReset;
        return Page();
    }

    public async Task<IActionResult> OnPostMarkAsReservedAsync(string Id, int queueNumber)
    {
        Attendees = await queueRepository.GetAttendees(Id.ToString());
        var attendee = Attendees?.FirstOrDefault(a => a?.QueueNumber == queueNumber);
        var validationResult = await validator.ValidateAsync(attendee);

        if (!validationResult.IsValid)
        {
            return new JsonResult(new { success = false });
        }

        await queueRepository.MarkAsReserved(attendee, queueNumber);

        Attendees = await queueRepository.GetAttendees(Id.ToString());
        var updatedHtml = RenderAttendeesTableBody(Attendees!);

        return new JsonResult(new { success = true, html = updatedHtml });
    }

    private string RenderAttendeesTableBody(List<QueueTicket> attendees)
    {
        var sb = new StringBuilder();

        foreach (var attendee in attendees)
        {
            sb.Append($"""
            <tr>
                <td>{attendee.QueueNumber}</td>
                <td>{attendee.CustomerName}</td>
                <td>{attendee.CustomerPhoneNumber}</td>
                <td>
                    <form method='post' id='markAsReservedForm-{attendee.QueueNumber}'>
                        <input type='hidden' name='Id' value='{Id}' />
                        <input type='hidden' name='queueNumber' value='{attendee.QueueNumber}' />
                        <button type='button' class='btn reserve-blue-button' onclick='confirmMarkAsReserved({attendee.QueueNumber})'>{L["mark-as-reserved-button"]}</button>
                    </form>
                </td>
            </tr>
            """);
        }

        return sb.ToString();
    }

    public async Task<IActionResult> OnPostResetAsync()
    {
        await queueRepository.ResetQueue(Id.ToString());
        Attendees = await queueRepository.GetAttendees(Id.ToString());
        var updatedHtml = RenderAttendeesTableBody(Attendees!);
        return new JsonResult(new { success = true, html = updatedHtml });
    }

    public async Task<IActionResult> OnGetFetchNewDataAsync(string Id)
    {
        Attendees = await queueRepository.GetAttendees(Id);
        var updatedHtml = RenderAttendeesTableBody(Attendees!);
        return new ContentResult
        {
            Content = updatedHtml,
            ContentType = "text/html",
            StatusCode = 200 // OK
        };
    }
}