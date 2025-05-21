using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TaludiaCalendarApp.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected readonly ILogger<ViewModelBase>? _logger;

        // Permite inyección opcional de logger
        public ViewModelBase(ILogger<ViewModelBase>? logger = null)
        {
            _logger = logger;
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            try
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en SetProperty para la propiedad {PropertyName}", name);
                throw;
            }
        }
    }
}
