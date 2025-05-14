using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TaludiaCalendar;

public static class GoogleApiClientFactory
{
    public static CalendarService CreateCalendarService()
    {
        using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
        string credPath = "token.json";

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { CalendarService.Scope.CalendarReadonly },
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;

        return new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Calendar API .NET 8 Example",
        });
    }
}
