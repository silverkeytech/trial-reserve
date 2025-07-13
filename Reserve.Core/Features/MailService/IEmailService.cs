namespace Reserve.Core.Features.MailService;

public interface IEmailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}