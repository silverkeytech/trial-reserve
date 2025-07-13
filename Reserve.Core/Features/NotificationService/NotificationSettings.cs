using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;

namespace Reserve.Core.Features.NotificationService
{
    public class NotificationSettings
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly ILogger<NotificationSettings> _logger;

        public NotificationSettings(FirebaseApp firebaseApp, ILogger<NotificationSettings> logger)
        {
            _firebaseApp = firebaseApp;
            _logger = logger;
        }

        public async Task<string> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                var message = new Message()
                {
                    Token = token,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data ?? new Dictionary<string, string>()
                    {
                        { "click_action", "WEB_NOTIFICATION_CLICK" },
                        { "url", "https://localhost:7187/" }
                    }
                };

                _logger.LogInformation("Sending notification to token: {Token}", token);
                
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                
                _logger.LogInformation("Notification sent successfully. Response: {Response}", response);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                throw;
            }
        }
    }
}
