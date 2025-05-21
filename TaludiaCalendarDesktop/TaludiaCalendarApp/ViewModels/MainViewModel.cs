using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TaludiaCalendarApp.Models;
using TaludiaCalendarApp.Services;
using Microsoft.Extensions.Logging;

namespace TaludiaCalendarApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CalendarService _calendarService;
        private readonly ILogger<MainViewModel> _logger;

        public MainViewModel(
            CalendarService calendarService,
            ILogger<MainViewModel> logger,
            ILogger<ViewModelBase> baseLogger
        ) : base(baseLogger)
        {
            _calendarService = calendarService;
            _logger = logger;
        }

        public ObservableCollection<CalendarEventDto> Events { get; } = new();
        public ObservableCollection<string> Obras { get; } = new();
        public ObservableCollection<string> Trabajadores { get; } = new();
        public ObservableCollection<string> Vehiculos { get; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public ICommand LoadEventsCommand => new RelayCommand(async () => await LoadEvents());

        private async Task LoadEvents()
        {
            try
            {
                Events.Clear();
                Obras.Clear();
                Trabajadores.Clear();
                Vehiculos.Clear();

                var eventos = await _calendarService.FetchEventsAsync(SelectedDate);
                foreach (var ev in eventos)
                {
                    if (ev == null) continue;
                    Events.Add(ev);
                    var parsed = ParseTitle(ev.Title ?? string.Empty);

                    if (!string.IsNullOrWhiteSpace(parsed.Obra) && !Obras.Contains(parsed.Obra))
                        Obras.Add(parsed.Obra);

                    foreach (var t in parsed.Trabajadores.Where(t => !string.IsNullOrWhiteSpace(t) && !Trabajadores.Contains(t)))
                        Trabajadores.Add(t);

                    foreach (var v in parsed.Vehiculos.Where(v => !string.IsNullOrWhiteSpace(v) && !Vehiculos.Contains(v)))
                        Vehiculos.Add(v);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al cargar los eventos en MainViewModel.");
            }
        }

        private (string Obra, List<string> Trabajadores, List<string> Vehiculos) ParseTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return ("", new List<string>(), new List<string>());

            var obraMatch = System.Text.RegularExpressions.Regex.Match(title, @"^(.*?)\s*(\(|\[|$)");
            var obra = obraMatch.Success ? obraMatch.Groups[1].Value.Trim() : "";

            var trabajadoresMatch = System.Text.RegularExpressions.Regex.Match(title, @"\(([^)]*)\)");
            var trabajadores = trabajadoresMatch.Success && !string.IsNullOrWhiteSpace(trabajadoresMatch.Groups[1].Value)
                ? trabajadoresMatch.Groups[1].Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToList()
                : new List<string>();

            var vehiculosMatch = System.Text.RegularExpressions.Regex.Match(title, @"\[([^\]]*)\]");
            var vehiculos = vehiculosMatch.Success && !string.IsNullOrWhiteSpace(vehiculosMatch.Groups[1].Value)
                ? vehiculosMatch.Groups[1].Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim()).ToList()
                : new List<string>();

            return (obra, trabajadores, vehiculos);
        }

    }
}
