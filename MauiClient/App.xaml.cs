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
    }

    private async void CheckAuthentication()
    {
        var appShell = MainPage as AppShell;
        if (appShell == null) return;

        appShell.IsAuthenticated = await new LoginService().IsAuthenticatedAsync();

        if (appShell.IsAuthenticated)
        {
            await Shell.Current.GoToAsync("HomePage");
        }
        else
        {
            await Shell.Current.GoToAsync("LoginPage");
        }
    }

}
