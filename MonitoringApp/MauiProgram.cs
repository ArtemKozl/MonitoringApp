using Microsoft.Extensions.Logging;
using MonitoringApp.Services;
using MonitoringApp.View;
using MonitoringApp.ViewModel;
using Syncfusion.Maui.Core.Hosting;

namespace MonitoringApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MonitoringNowPage>();
            builder.Services.AddSingleton<ComparePage>();
            builder.Services.AddSingleton<MonitoringMethods>();
            builder.Services.AddSingleton<AllMonitoringValuesViewModel>();
            return builder.Build();
        }
    }
}
