using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using WPFClient.Helpers;
using WPFClient.Services;
using WPFClient.Views;
using BaseLibrary.DTOs;
using System.Windows;

namespace WPFClient.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly IMyNavigationService _navigationService;
        private string _name;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _errorMessage;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel(IAuthenticationService authService, IMyNavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            RegisterCommand = new RelayCommand(ExecuteRegisterAsync, CanExecuteRegister);
            NavigateToLoginCommand = new RelayCommand(ExecuteNavigateToLogin);
        }

        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   Password == ConfirmPassword;
        }

        private async void ExecuteRegisterAsync()
        {
            try
            {

                var registerDto = new RegisterUserDTO
                {
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword
                };

                var result = await _authService.RegisterAsync(registerDto);
                if (result)
                {
                    _navigationService.NavigateTo<LoginView>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ExecuteNavigateToLogin()
        {
            _navigationService.NavigateTo<LoginView>();
        }
    }
}
