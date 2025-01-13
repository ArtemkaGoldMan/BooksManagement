using BaseLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;


namespace MauiClient.ViewModel
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly RegistrationService _registrationService;

        [ObservableProperty] private string name;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private string confirmPassword;
        [ObservableProperty] private string errorMessage;

        public RegistrationViewModel(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "All fields are required.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            var registerDto = new RegisterUserDTO
            {
                Name = Name,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword
            };

            var result = await _registrationService.RegisterAsync(registerDto);

            if (!result)
            {
                ErrorMessage = "Registration failed. Please try again.";
                return;
            }

            // Navigate to the login page after successful registration
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            // Use absolute routing to ensure navigation works
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
