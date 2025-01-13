using MauiClient.Services;

namespace MauiClient;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;

        MainPage = new AppShell();

        // Check authentication on app startup
        CheckAuthentication();
    }

    private async void CheckAuthentication()
    {
        try
        {
            // Retrieve the LoginService
            var loginService = Services.GetService<LoginService>();
            if (loginService == null)
            {
                throw new InvalidOperationException("LoginService is not registered in the service container.");
            }

            // Check authentication state
            if (MainPage is not AppShell appShell) return;

            appShell.IsAuthenticated = await loginService.IsAuthenticatedAsync();

            // Navigate based on authentication state
            if (appShell.IsAuthenticated)
            {
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during authentication check: {ex.Message}");
        }
    }

}
