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
    public async Task LoginAsync()
    {
        try
        {
            Console.WriteLine($"Attempting login with Email: {Email}");

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
                appShell.IsAuthenticated = true;
            }

            // Navigate to the Home Page
            await Shell.Current.GoToAsync("/HomePage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in LoginViewModel.LoginAsync: {ex.Message}");
            ErrorMessage = "An error occurred during login.";
        }

    }

    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//RegistrationPage");
    }

}
