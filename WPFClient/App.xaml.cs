using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Navigation;
using WPFClient.Services;
using WPFClient.ViewModels;
using WPFClient.Helpers;
using WPFClient.Views;
using Microsoft.Extensions.DependencyInjection;
namespace WPFClient
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IMyNavigationService, MyNavigationService>();
            services.AddSingleton<IBooksService, BooksService>();



            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            //services.AddTransient<BooksViewModel>();
            services.AddTransient<BooksViewModel>();
            //services.AddScoped<EditBookViewModel>();
            services.AddTransient<AdminBooksViewModel>();
            services.AddTransient<AddBookViewModel>();
            services.AddTransient<AdminBorrowedBooksViewModel>();



            // Register Views
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<BooksWindow>();
            services.AddTransient<BorrowedBooksWindow>();
            services.AddTransient<EditBookView>();
            services.AddTransient<AddBookView>();
            services.AddTransient<AdminBooksWindow>();
            services.AddTransient<AdminBorrowedBooksWindow>();
        }
        /*protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
        }*/


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up unhandled exception handling
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
        }


        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

    }
}