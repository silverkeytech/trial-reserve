using FluentValidation;
using Microsoft.Extensions.Localization;
using Reserve.Core;
namespace Reserve.Core.Features.Queue;

public class QueueEvent
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string OrganizerEmail { get; set; }
    public required string Description { get; set; }
    public int CurrentNumberServed { get; set; }
    public int TicketCounter { get; set; }
    public DateTime LastReset { get; set; }
}

public class QueueEventValidator : AbstractValidator<QueueEvent>
{
    public QueueEventValidator(IStringLocalizer<Global>L)
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage(L["validation:name-required"]).Length(2, 15).WithMessage(L["validation:name-length"]); ;
        RuleFor(x => x.OrganizerEmail)
            .NotEmpty().WithMessage(L["validation:email-required"])
            .EmailAddress().WithMessage("validation:email-format");
        RuleFor(x => x.Description).NotEmpty().WithMessage(L["validation:email-required"]).Length(10, 100).WithMessage(L["validation:descritption-length"]); ;
        RuleFor(x => x.CurrentNumberServed).NotNull().WithMessage("Current Number Served is required").GreaterThanOrEqualTo(0).WithMessage("Current Number Served must be 0 or greater");
        RuleFor(x => x.TicketCounter).NotNull().WithMessage("Ticket Counter is required").GreaterThanOrEqualTo(0).WithMessage("Ticket Counter must be 0 or greater");
    }
}