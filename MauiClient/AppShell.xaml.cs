using MauiClient.Services;
using MauiClient.Views;

namespace MauiClient;

public partial class AppShell : Shell
{
    private readonly LoginService _loginService;
    private readonly ToolbarItem _logoutToolbarItem;

    public static readonly BindableProperty IsAuthenticatedProperty =
        BindableProperty.Create(nameof(IsAuthenticated), typeof(bool), typeof(AppShell), false, propertyChanged: OnIsAuthenticatedChanged);

    public bool IsAuthenticated
    {
        get => (bool)GetValue(IsAuthenticatedProperty);
        set => SetValue(IsAuthenticatedProperty, value);
    }

    public AppShell()
    {
        InitializeComponent();

        // Retrieve the LoginService from the DI container
        _loginService = App.Services.GetService<LoginService>();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));

        // Create the Logout button
        _logoutToolbarItem = new ToolbarItem
        {
            Text = "Logout",
            Command = new Command(async () => await HandleLogoutAsync())
        };

        // Add or remove the Logout button based on authentication state
        UpdateToolbar();
    }

    private async Task HandleLogoutAsync()
    {
        await _loginService.LogoutAsync();
        SecureStorage.Default.Remove("JwtToken");

        // Clear authentication state
        IsAuthenticated = false;

        // Navigate to the Login Page
        await Shell.Current.GoToAsync("//LoginPage");
    }


    private void UpdateToolbar()
    {
        if (IsAuthenticated)
        {
            if (!ToolbarItems.Contains(_logoutToolbarItem))
            {
                ToolbarItems.Add(_logoutToolbarItem);
            }
        }
        else
        {
            if (ToolbarItems.Contains(_logoutToolbarItem))
            {
                ToolbarItems.Remove(_logoutToolbarItem);
            }
        }
    }

    private static void OnIsAuthenticatedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AppShell shell)
        {
            shell.UpdateToolbar();
        }
    }
}
