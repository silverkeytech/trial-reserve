using EdgeDB;
using FluentValidation;
using Reserve.Core.Features.Queue;
using Reserve.Core.Features.Event;
using Reserve.Core.Features.MailService;
using Reserve.Core.Features.reCaptcha;
using Reserve.Endpoints;
using Reserve.Core.Features.Appointment;
using SoloX.BlazorJsonLocalization;
using Reserve.Core;
using System.Reflection;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Reserve.Core.Features.NotificationService;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddJsonLocalization(builder =>
      builder
        .UseEmbeddedJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.Assemblies = new[] { typeof(Global).Assembly, typeof(AppointmentLocalization).Assembly, Assembly.GetExecutingAssembly() };
        })
        .AddFallback("Global", typeof(Global).Assembly)
        .AddFallback("AppointmentLocalization", typeof(AppointmentLocalization).Assembly)
        .EnableLogger(true)
        , ServiceLifetime.Singleton
);

// Add services to the container.
services.AddRazorPages().AddMvcOptions(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
               _ => "This field is required.");
});
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration.GetConnectionString("EdgeDB");
var connection = EdgeDBConnection.FromDSN(connectionString!);
connection.TLSCertificateAuthority = "insecure";

services.AddEdgeDB(connection, config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

services.AddScoped<IEventRepository, EventRepository>();
services.AddScoped<IQueueRepository, QueueRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();
services.AddScoped<IValidator<QueueEvent>, QueueEventValidator>();
services.AddScoped<IValidator<QueueTicket>, QueueTicketValidator>();
services.AddScoped<IValidator<CasualEventInput>, CasualEventInputValidator>();
services.AddScoped<IValidator<CasualTicketInput>, CasualTicketInputValidator>();
services.AddScoped<IValidator<AppointmentDetails>, AppointmentDetailsValidator>();
services.AddScoped<IValidator<AppointmentCalendar>, AppointmentCalendarValidator>();
services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
services.AddTransient<IEmailService, EmailService>();
services.AddTransient<IReCaptchaService, ReCaptchaService>();
services.AddSingleton<ReCaptchaSettings, ReCaptchaSettings>();

services.AddSingleton<FirebaseApp>(sp =>
{
    return FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile("private_key.json")
    });
});
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

services.AddTransient<NotificationSettings>();

ConfigureCulture(services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();

app.UseRouting();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorPages();

app.MapGroup("/").MapEventsApi();
app.MapGroup("/").MapAppointmentsApi();
app.Run();

void ConfigureCulture(IServiceCollection services)
{
    var english = new CultureInfo("en");
    english.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";

    var arabic = new CultureInfo("ar", true);
    arabic.DateTimeFormat = new CultureInfo("en").DateTimeFormat;
    arabic.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

    var supportedCultures = new CultureInfo[]
    {
        english,
        arabic
    };

    services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(supportedCultures[1]); //arabic
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders.Clear();
        options.RequestCultureProviders.Add(new CustomRequestCultureProvider(context =>
        {
            var defaultCulture = options.DefaultRequestCulture.Culture;

            var segments = context.Request.Path.Value?.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments?.Length > 0)
            {
                foreach (var c in options.SupportedCultures)
                {
                    if (c.Name.Equals(segments[0], StringComparison.InvariantCulture))
                        return Task.FromResult(new ProviderCultureResult(c.Name))!;
                }
            }

            return Task.FromResult(default(ProviderCultureResult));
        }));

        options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
    });
}

