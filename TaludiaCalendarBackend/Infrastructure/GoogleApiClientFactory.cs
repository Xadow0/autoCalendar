using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
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
        var dataStore = new FileDataStore(credPath, true);
        UserCredential credential;

        try
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { CalendarService.Scope.CalendarReadonly },
                "user",
                CancellationToken.None,
                dataStore).Result;
        }
        catch (TokenResponseException ex) when (ex.Error != null && ex.Error.Error == "invalid_grant")
        {
            // Token inválido o revocado, borramos y pedimos nueva autenticación
            dataStore.ClearAsync().Wait();

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { CalendarService.Scope.CalendarReadonly },
                "user",
                CancellationToken.None,
                dataStore).Result;
        }

        return new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Calendar API .NET 8 Example",
        });
    }
}
