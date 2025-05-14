using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using TaludiaCalendar.Models;

namespace TaludiaCalendar.Services;

public interface IGoogleCalendarService
{
    Task<IEnumerable<CalendarEventDto>> GetWeeklyEventsAsync();
}

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly CalendarService _calendarService;

    public GoogleCalendarService()
    {
        _calendarService = GoogleApiClientFactory.CreateCalendarService();
    }

    public async Task<IEnumerable<CalendarEventDto>> GetWeeklyEventsAsync()
    {
        var request = _calendarService.Events.List("primary");
        var today = DateTime.Today;
        request.TimeMinDateTimeOffset = today;
        request.TimeMaxDateTimeOffset = today.AddDays(7);
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
