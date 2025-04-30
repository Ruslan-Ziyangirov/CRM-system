using Microsoft.Extensions.Logging;


namespace CRM_system;
using CRM_system.Services;
using CRM_system.ViewModels;
using CRM_system.Views;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register database as singleton
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystore.db3");
        builder.Services.AddSingleton<AppDatabase>(s => ActivatorUtilities.CreateInstance<AppDatabase>(s, dbPath));

        // Register ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<CreateDealPage>();
        builder.Services.AddTransient<CreateDealViewModel>();

        return builder.Build();
    }
}
