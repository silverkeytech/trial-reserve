using FluentValidation;
using Microsoft.Extensions.Localization;
using Reserve.Core;
namespace Reserve.Core.Features.Queue;

public class QueueTicket
{
    public Guid Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public int QueueNumber { get; set; }
    public QueueEvent? QueueEvent { get; set; }
    public string? Status { get; set; }
}

public class QueueTicketValidator : AbstractValidator<QueueTicket>
{
    public QueueTicketValidator(IStringLocalizer<Global> L)
    {
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage(L["validation:name-required"]);
        RuleFor(x => x.CustomerPhoneNumber).NotEmpty().WithMessage(L["validation:phone-number-required"])
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(L["validation:phone-number-format"])
            .Length(10, 16).WithMessage(L["validation:phone-number-format"]);
        RuleFor(x => x.QueueNumber).NotNull().WithMessage("Queue Number is required").GreaterThanOrEqualTo(0).WithMessage("Queue Number must be 0 or greater");
        RuleFor(x => x.QueueEvent).NotNull().WithMessage("Queue Event doesnt exist");
    }
}