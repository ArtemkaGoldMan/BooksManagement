using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;
using MauiClient.Views;

namespace MauiClient.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    private readonly LoginService _loginService;

    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string errorMessage;

    public LoginViewModel(LoginService loginService)
    {
        _loginService = loginService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            var token = await _loginService.LoginAsync(new BaseLibrary.DTOs.LoginUserDTO
            {
                Email = Email,
                Password = Password
            });

            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Invalid credentials. Please try again.";
                return;
            }

            // Update authentication state
            if (App.Current.MainPage is AppShell appShell)
            {
                appShell.IsAuthenticated = true; // Show the TabBar and Logout button
            }

            // Navigate to the Home Page
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//RegistrationPage");
    }

}
