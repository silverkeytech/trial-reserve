using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core.Features.NotificationService;
using Reserve.Core.Features.Queue;
using Reserve.Helpers;

namespace Reserve.Pages.Queue;

[BindProperties]
public class QueueTicketModel(IQueueRepository? queueRepository, IStringLocalizer<QueueLocalization> L, NotificationSettings notificationSettings) : PageModel
{
    [BindProperty]
    public string? FcmToken { get; set; }

    public string? CurrentUrl { get; set; }
    public string? QueueTicketId { get; set; }
    public string? QueueId { get; set; }

    public QueueEventView? CurrentQueueView { get; set; }
    public QueueTicketView? QueueTicketView { get; set; }

    public void OnGet(string QueueTicketId, string QueueId)
    {
        this.QueueTicketId = QueueTicketId;
        this.QueueId = QueueId;
    }

    public async Task<IActionResult> OnGetRenderTicketAsync(string QueueTicketId, string QueueId)
    {
        Guid QueueTicketid = GuidShortener.RestoreGuid(QueueTicketId);
        Guid Queueid = GuidShortener.RestoreGuid(QueueId);
        CurrentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
        QueueEvent? CurrentQueue = await queueRepository!.GetQueueEventByID(Queueid.ToString());
        QueueTicket? QueueTicket = await queueRepository!.GetQueueTicketByID(QueueTicketid.ToString());

        if (CurrentQueue == null || QueueTicket == null)
        {
            string notFoundMessage = L["not-found-message"];
            return new ContentResult
            {
                Content = $"<div class='alert alert-danger' role='alert'>{notFoundMessage}</div>",
                ContentType = "text/html"
            };
        }
        string queueTitle = L["queue-ticket-title"];
        string queueNumber = L["queue-number", QueueTicket.QueueNumber];
        string currentNumberServed = L["current-number-served", CurrentQueue.CurrentNumberServed];
        string inQueueMessage = L["in-queue-message"];
        string html;

        if (string.IsNullOrEmpty(QueueTicket.Status))
        {
            html = $"""
            <div class="container mt-5">
                <h1 class="display-4 text-center">{queueTitle}</h1>
                <div class="card border rounded mt-4">
                    <div class="card-body">
                        <h3 class="card-title text-center">{L["queue-welcome-message", CurrentQueue.Title]}</h3>
                        <br />
                        <div class="text-center">
                            <p class="card-text" style="font-size: 2rem; font-weight: bold;">{queueNumber}</p>
                        </div>
                        <p class="card-text text-center mt-3">{inQueueMessage}</p>
                    </div>
                    <div class="card-footer">
                        <p class="text-muted text-center">{currentNumberServed}</p>
                    </div>
                </div>
            </div>
        """;
        }
        else if (QueueTicket.Status == "served")
        {
            string alreadyServedAlert = L["already-served-alert"];
            string alreadyServedMessage = L["already-served-message"];

            html = $"""
            <div class="alert alert-success" role="alert">{alreadyServedAlert}</div>
            <div class="container mt-5">
                <h1 class="display-4 text-center">{queueTitle}</h1>
                <div class="card border rounded mt-4">
                    <div class="card-body">
                        <h3 class="card-title text-center">{L["queue-welcome-message", CurrentQueue.Title]}</h3>
                        <br />
                        <div class="text-center">
                            <p class="card-text" style="font-size: 2rem; font-weight: bold;">{queueNumber}</p>
                        </div>
                        <p class="card-text text-center mt-3">{alreadyServedMessage}</p>
                    </div>
                    <div class="card-footer">
                        <p class="text-muted text-center">{currentNumberServed}</p>
                    </div>
                </div>
            </div>
        """;
        }
        else if (QueueTicket.Status == "reset")
        {
            string resetAlert = L["reset-alert"];
            string resetWarningMessage = L["reset-warning-message"];

            html = $"""
            <div class="alert alert-warning" role="alert">{resetAlert}</div>
            <div class="container mt-5">
                <h1 class="display-4 text-center">{queueTitle}</h1>
                <div class="card border rounded mt-4">
                    <div class="card-body">
                        <h3 class="card-title text-center">{L["queue-welcome-message", CurrentQueue.Title]}</h3>
                        <br />
                        <div class="text-center">
                            <p class="card-text" style="font-size: 2rem; font-weight: bold;">{queueNumber}</p>
                        </div>
                        <p class="card-text text-center mt-3">{inQueueMessage}</p>
                    </div>
                    <div class="card-footer">
                        <p class="text-muted text-center">{currentNumberServed}</p>
                    </div>
                </div>
            </div>
        """;
        }
        else
        {
            string invalidTicketStatus = L["invalid-ticket-status"];
            html = $"""<div class="alert alert-warning" role="alert">{invalidTicketStatus}</div>""";
        }
        if (CurrentQueue.TicketCounter == QueueTicket.QueueNumber)
        {
            var title = L["first-in-line"];
            var body = L["please-go-to-counter"];
            Dictionary<string, string> data = new Dictionary<string, string>()
        {
             { "click_action", "WEB_NOTIFICATION_CLICK" },
             { "tag", "notification-tag" },
             { "url", "" }
        };
            if (FcmToken != null)
            {
                string response = await notificationSettings.SendNotificationAsync(FcmToken, title, body, data);
            }
        }
        return new ContentResult { Content = html, ContentType = "text/html" };
    }
}