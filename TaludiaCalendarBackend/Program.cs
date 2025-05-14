using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint: GET /calendar/events
app.MapGet("/calendar/events", async () =>
{
    UserCredential credential;

    using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
    {
        string credPath = "token.json";

        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { CalendarService.Scope.CalendarReadonly },
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true));
    }

    var service = new CalendarService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = credential,
        ApplicationName = "Google Calendar API .NET 8 Example",
    });

    var request = service.Events.List("primary");
    var today = DateTime.Today;
    request.TimeMinDateTimeOffset = today;
    request.TimeMaxDateTimeOffset = today.AddDays(7);
    request.ShowDeleted = false;
    request.SingleEvents = true;
    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

    Events events = await request.ExecuteAsync();

    var results = events.Items.Select(e => new
    {
        Title = e.Summary,
        Start = e.Start.DateTimeDateTimeOffset?.ToString("o") ?? e.Start.Date,
        End = e.End.DateTimeDateTimeOffset?.ToString("o") ?? e.End.Date
    });

    return Results.Ok(results);
});

app.Run();
