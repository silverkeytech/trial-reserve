using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Reserve.Core.Features.Appointment;

public class AppointmentCalendar
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public List<Availability> AvailabilitySlots { get; set; } = [];
}

public class AppointmentCalendarValidator : AbstractValidator<AppointmentCalendar>
{
    public AppointmentCalendarValidator(IStringLocalizer<Global> L)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(L["validation:name-required"]);
        RuleFor(x => x.Email).NotEmpty().WithMessage(L["validation:email-required"]).EmailAddress().WithMessage(L["validation:email-format"]);
        RuleFor(x => x.Description).NotEmpty().WithMessage(L["validation:description-required"]).Length(10, 100).WithMessage(L["validation:description-length"]);
    }
}