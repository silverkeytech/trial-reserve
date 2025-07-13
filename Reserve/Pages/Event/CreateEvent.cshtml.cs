using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Reserve.Core;
using Reserve.Core.Features.Event;
using Reserve.Core.Features.MailService;
using Reserve.Core.Features.reCaptcha;
using Reserve.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static Reserve.Core.Features.MailService.MailFormats;
using static Reserve.Helpers.ImageHelper;
namespace Reserve.Pages.Event;

[BindProperties]
public class CreateEventModel(IEventRepository eventRepository, IWebHostEnvironment webHostEnvironment, IValidator<CasualEventInput> validator, IEmailService emailService, ReCaptchaSettings reCaptchaSettings, IReCaptchaService reCaptchaService, IStringLocalizer<Global> localizer) : PageModel
{
    public CasualEventInput? NewEvent { get; set; }

    public string ReCaptchaSiteKey => reCaptchaSettings.SiteKey!;

    public string? ReCaptchaToken { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (NewEvent is null)
        {
            return RedirectToPage("EventError");
        }
        IFormFile? imageFile = Request.Form.Files.FirstOrDefault();
        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(NewEvent);
        var reCaptchaResult = await reCaptchaService.VerifyReCaptchaAsync(ReCaptchaToken!, reCaptchaSettings);
        if (imageFile is not null)
        {
            NewEvent.ImageUrl = @"\images\event\" + imageFile.FileName;
        }
        else
        {
            NewEvent.ImageUrl = @"\images\generic.jpeg";
        }
        if (result.IsValid)
        {
            if (imageFile is not null)
            {
                NewEvent.ImageUrl = SaveImage(imageFile, webHostEnvironment);
                string imagePath = Path.Combine(webHostEnvironment.WebRootPath, NewEvent.ImageUrl);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/event"));
                string fullPath = Path.Combine(path, Path.GetFileName(NewEvent.ImageUrl));
                if (imageFile.Length >= 3 * 1024 * 1024)
                {
                    using (Image image = Image.Load(imageFile.OpenReadStream()))
                    {
                        int width = image.Width / 2;
                        int height = image.Height / 2;
                        image.Mutate(x => x.Resize(width, height));
                        image.Save(fullPath);
                    }
                }
            }
            CasualEvent? casualEvent = new CasualEvent
            {
                Title = NewEvent.Title,
                OrganizerName = NewEvent.OrganizerName,
                OrganizerEmail = NewEvent.OrganizerEmail,
                MaximumCapacity = NewEvent.MaximumCapacity,
                CurrentCapacity = NewEvent.CurrentCapacity,
                Location = NewEvent.Location,
                StartDate = NewEvent.StartDate,
                EndDate = NewEvent.EndDate,
                Opened = NewEvent.Opened,
                Description = NewEvent.Description,
                ImageUrl = NewEvent.ImageUrl
            };
            casualEvent = await eventRepository.CreateAsync(casualEvent);
            if (casualEvent is not null)
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = NewEvent.OrganizerEmail,
                    Subject = "Event Created",
                    Body = EventCreationNotification(GuidShortener.ShortenGuid(casualEvent.Id), baseUrl)
                };
                await emailService.SendEmailAsync(mailRequest);
                return RedirectToPage("CreationNotification", new { id = GuidShortener.ShortenGuid(casualEvent.Id) });
            }
            else
            {
                return RedirectToPage("EventError");
            }
        }
        else if (!result.IsValid || !reCaptchaResult)
        {
            if (!reCaptchaResult)
            {
                ModelState.AddModelError("ReCaptchaToken", localizer["validation:recaptcha-required"]);
            }
            result.AddToModelState(ModelState, "NewEvent");
        }
        return Page();
    }
}