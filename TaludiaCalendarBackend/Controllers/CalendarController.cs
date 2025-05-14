using TaludiaCalendar.Services;

public static class CalendarController
{
    public static void MapCalendarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/calendar/events", async (IGoogleCalendarService calendarService) =>
        {
            var events = await calendarService.GetWeeklyEventsAsync();
            return Results.Ok(events);
        });
    }
}
