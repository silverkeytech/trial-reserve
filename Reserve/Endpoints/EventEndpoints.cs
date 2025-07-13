using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Reserve.Core.Features.Event;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;
using Reserve.Helpers;
namespace Reserve.Endpoints;

public static class EventEndpoints
{
    public static RouteGroupBuilder MapEventsApi(this RouteGroupBuilder group)
    {
        group.MapPost("close-reservation/{id}", async ([FromRoute] string id, HttpContext context, IEventRepository _eventRepository, IAntiforgery antiforgery) =>
        {
            try
            {
                await _eventRepository.CloseReservationAsync(GuidShortener.RestoreGuid(id).ToString());
                var html = @"<p class=""notification-text m-3"">Reservation closed!</p>";
                return Results.Content(html, "text/html");
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapDelete("cancel-reservation/{id}", async([FromRoute] string id, HttpContext context, IEventRepository _eventRepository, IAntiforgery antiforgery, IEmailService _emailService) =>
        {
            try
            {
                CasualTicket? casualTicket = await _eventRepository.GetTicketByIdAsync(GuidShortener.RestoreGuid(id).ToString());
                ArgumentNullException.ThrowIfNull(casualTicket);
                ArgumentNullException.ThrowIfNull(casualTicket.CasualEvent);
                Guid guidTicketId = GuidShortener.RestoreGuid(id);

                CasualEvent? eventowner = await _eventRepository.GetByIdAsync(casualTicket.CasualEvent.Id.ToString());

                if (eventowner != null)
                {
                    string baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                    var ownerEmail = new MailRequest
                    {
                        ToEmail = eventowner.OrganizerEmail,
                        Subject = "Event reservation cancelled!",
                        Body = EventCancellationNotificationOwner(GuidShortener.ShortenGuid(eventowner.Id).ToString(),baseUrl)
                    };
                    await _emailService.SendEmailAsync(ownerEmail);
                }

                await _eventRepository.CancelReservationAsync(guidTicketId, casualTicket.CasualEvent.Id);
                context.Response.Headers["HX-Redirect"] = "/event-cancellation";
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        return group;
    }
}
