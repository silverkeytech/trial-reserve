using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Reserve.Core.Features.Appointment;

public class AppointmentDetails
{
    public Guid Id { get; set; }
    public string? ReserverName { get; set; }
    public string? ReserverEmail { get; set; }
    public string? ReserverPhoneNumber { get; set; }
    public Availability? Slot { get; set; }
    public AppointmentState AppointmentStatus { get; set; }

    public bool IsSlotAvailable()
    {
        return Slot?.Available ?? false;
    }
}
public enum AppointmentState
{
    Pending,
    Done
}

public class AppointmentDetailsValidator : AbstractValidator<AppointmentDetails>
{
    public AppointmentDetailsValidator(IStringLocalizer<Global> localizer)
    {
        RuleFor(x => x.ReserverName)
            .NotEmpty()
            .WithMessage(localizer["validation:name-required"]);

        RuleFor(x => x.ReserverEmail)
            .NotEmpty()
            .WithMessage(localizer["validation:email-required"])
            .EmailAddress()
            .WithMessage(localizer["validation:email-format"]);

        RuleFor(x => x.ReserverPhoneNumber)
            .NotEmpty()
            .WithMessage(localizer["validation:phone-number-required"])
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage(localizer["validation:phone-number-format"])
            .Length(10, 16)
            .WithMessage(localizer["validation:phone-number-format"]);

        RuleFor(x => x.Slot)
            .Must(slot => slot != null && slot.Available)
            .WithMessage(localizer["validation:slot-already-reserved"]);
    }
}