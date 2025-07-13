using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Reserve.Core.Features.Event;

public class CasualEventInput
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerEmail { get; set; }
    public int? MaximumCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool Opened { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class CasualEventInputValidator : AbstractValidator<CasualEventInput>
{
    public CasualEventInputValidator(IStringLocalizer<Global> eL)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(eL["validation:title-required"])
            .Length(2, 15).WithMessage(eL["validation:title-length"]);

        RuleFor(x => x.OrganizerName)
            .NotEmpty().WithMessage(eL["validation:organizer-name-required"]);

        RuleFor(x => x.OrganizerEmail)
            .NotEmpty().WithMessage(eL["validation:organizer-email-required"])
            .EmailAddress().WithMessage(eL["validation:email-format"]);

        RuleFor(x => x.MaximumCapacity)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(eL["validation:maximum-capacity-required"])
            .NotEqual(0).WithMessage(eL["validation:maximum-capacity-zero"]);

        RuleFor(x => x.MaximumCapacity)
            .GreaterThanOrEqualTo(x => x.CurrentCapacity)
            .WithMessage(eL["validation:maximum-capacity-gt-current"]);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage(eL["validation:location-required"]);

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage(eL["validation:end-date-required"]);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(eL["validation:description-required"])
            .Length(10, 100).WithMessage(eL["validation:description-length"]);

        RuleFor(x => x.ImageUrl)
            .Must(HaveValidImageExtension).WithMessage(eL["validation:image-extension"]);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage(eL["validation:start-date-required"])
            .GreaterThan(DateTime.Now.AddHours(1)).WithMessage(eL["validation:start-date-future"])
            .LessThan(x => x.EndDate).WithMessage(eL["validation:start-date-before-end-date"]);
    }

    private bool HaveValidImageExtension(string? imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            string[] validExtensions = { ".jpeg", ".jpg", ".png" };
            string extension = Path.GetExtension(imageUrl).ToLower();
            return validExtensions.Contains(extension);
        }
        return true;
    }
}