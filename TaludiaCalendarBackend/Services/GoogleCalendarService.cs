using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using TaludiaCalendar.Models;

namespace TaludiaCalendar.Services;

public interface IGoogleCalendarService
{
    Task<IEnumerable<CalendarEventDto>> GetWeeklyEventsAsync(DateTime date);
}

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly CalendarService _calendarService;

    public GoogleCalendarService()
    {
        _calendarService = GoogleApiClientFactory.CreateCalendarService();
    }

    public async Task<IEnumerable<CalendarEventDto>> GetWeeklyEventsAsync(DateTime date)
{
    // Calcular el lunes de la semana correspondiente
    int daysToMonday = ((int)date.DayOfWeek + 6) % 7;
    DateTime monday = date.Date.AddDays(-daysToMonday);
    DateTime sunday = monday.AddDays(7);

    var request = _calendarService.Events.List("primary");
    request.TimeMinDateTimeOffset = new DateTimeOffset(monday);
    request.TimeMaxDateTimeOffset = new DateTimeOffset(sunday);
    request.ShowDeleted = false;
    request.SingleEvents = true;
    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

    Events events = await request.ExecuteAsync();

    return events.Items.Select(e => new CalendarEventDto
    {
        Title = e.Summary,
        Start = e.Start.DateTimeDateTimeOffset?.ToString("o") ?? e.Start.Date,
        End = e.End.DateTimeDateTimeOffset?.ToString("o") ?? e.End.Date
    });
}
}
