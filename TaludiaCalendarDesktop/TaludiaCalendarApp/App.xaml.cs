using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Windows;
using TaludiaCalendarApp.Services;
using TaludiaCalendarApp.ViewModels;
using TaludiaCalendarApp.Views;

namespace TaludiaCalendarApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Configurar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Registrar servicios y ViewModels
        services.AddSingleton<CalendarService>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>(); 

        // Base ViewModel no necesita registrar directamente si no se instancia por sí sola

        ServiceProvider = services.BuildServiceProvider();

        // Crear y mostrar la ventana principal
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }
}


