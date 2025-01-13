using MauiClient.Services;
using MauiClient.ViewModel;
using MauiClient.Views;
using Microsoft.Extensions.Logging;

namespace MauiClient
{
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

            // Register services and view models
            builder.Services.AddSingleton<TokenService>();

            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();

            builder.Services.AddSingleton<RegistrationService>();
            builder.Services.AddSingleton<RegistrationViewModel>();
            builder.Services.AddSingleton<RegistrationPage>();

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomePage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
