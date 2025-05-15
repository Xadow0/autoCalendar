using TaludiaCalendar.Services;
using System.Globalization;

public static class CalendarController
{
    public static void MapCalendarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/calendar/events", async (HttpRequest request, IGoogleCalendarService calendarService) =>
        {
            string? dateString = request.Query["date"];

            if (string.IsNullOrWhiteSpace(dateString) || 
                !DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return Results.BadRequest("Invalid or missing 'date' query parameter. Expected format: YYYY-MM-DD.");
            }

            var events = await calendarService.GetWeeklyEventsAsync(date);
            return Results.Ok(events);
        });
    }
}
