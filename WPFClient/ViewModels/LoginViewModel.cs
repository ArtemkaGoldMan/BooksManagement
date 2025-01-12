using Microsoft.Win32;
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

namespace WPFClient.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly IMyNavigationService _navigationService;
        private string _email;
        private string _password;
        private string _errorMessage;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public ICommand NavigateToBooksCommand { get; }

        public LoginViewModel(IAuthenticationService authService, IMyNavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            LoginCommand = new RelayCommand(ExecuteLoginAsync, CanExecuteLogin);
            NavigateToRegisterCommand = new RelayCommand(ExecuteNavigateToRegister);

        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        public async void ExecuteLoginAsync()
        {
            try
            {
                var loginDto = new LoginUserDTO { Email = Email, Password = Password };
                var result = await _authService.LoginAsync(loginDto);

                if (TokenStorage.IsAuthenticated)
                {
                    _navigationService.NavigateTo<BooksWindow>();
                }
                else
                {
                    ErrorMessage = "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ExecuteNavigateToRegister()
        {
            _navigationService.NavigateTo<RegisterView>();
        }

    }
}
