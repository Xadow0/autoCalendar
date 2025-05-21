using System.Net.Http;
using System.Text.Json;
using TaludiaCalendarApp.Models;
using Microsoft.Extensions.Logging;

namespace TaludiaCalendarApp.Services
{
    public class CalendarService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger<CalendarService>? _logger;

        // Permite inyección opcional de logger
        public CalendarService(ILogger<CalendarService>? logger = null)
        {
            _logger = logger;
        }

        public async Task<List<CalendarEventDto>> FetchEventsAsync(DateTime date)
        {
            try
            {
                string url = $"http://localhost:5256/calendar/events?date={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var eventos = JsonSerializer.Deserialize<List<CalendarEventDto>>(json);
                return eventos ?? new List<CalendarEventDto>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al obtener eventos para la fecha {Date}", date);
                throw;
            }
        }
    }
}
