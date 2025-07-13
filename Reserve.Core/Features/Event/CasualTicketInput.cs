using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Reserve.Core.Features.Event;

public class CasualTicketInput
{
    public Guid Id { get; set; }
    public string? ReserverName { get; set; }
    public string? ReserverEmail { get; set; }
    public string? ReserverPhoneNumber { get; set; }
    public CasualEvent? CasualEvent { get; set; }
}

public class CasualTicketInputValidator : AbstractValidator<CasualTicketInput>
{
    public CasualTicketInputValidator(IStringLocalizer<Global>localizer)
    {
        RuleFor(x => x.ReserverName).NotEmpty().WithMessage(localizer["validation:name-required"]);
        RuleFor(x => x.ReserverEmail).NotEmpty().WithMessage(localizer["validation:email-required"]).EmailAddress().WithMessage(localizer["validation:validation:email-format"]);
        RuleFor(x => x.ReserverPhoneNumber)
            .NotEmpty().WithMessage(localizer["validation:phone-number-required"])
            .Matches(@"^\+?[1-9]\d{1,15}$").WithMessage(localizer["validation:phone-number-format"]);
        RuleFor(x => x.CasualEvent).NotNull().Must(eventObj => eventObj?.Opened == true).WithMessage(localizer["validation:event-closed"]);
        RuleFor(x => x.CasualEvent).NotNull().Must(eventObj => eventObj?.CurrentCapacity < eventObj?.MaximumCapacity).WithMessage(localizer["validation:event-full"]);
    }
}