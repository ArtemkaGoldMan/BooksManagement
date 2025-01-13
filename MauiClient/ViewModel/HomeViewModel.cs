using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;

namespace MauiClient.ViewModel;

public partial class HomeViewModel : ObservableObject
{
    private readonly LoginService _loginService;

    public HomeViewModel(LoginService loginService)
    {
        _loginService = loginService;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _loginService.LogoutAsync();

        // Clear the authentication state in AppShell
        if (App.Current.MainPage is AppShell appShell)
        {
            appShell.IsAuthenticated = false;
        }

        // Navigate to the Login Page
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
