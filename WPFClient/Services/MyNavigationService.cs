using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace WPFClient.Services
{
    public class MyNavigationService : IMyNavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        public MyNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void NavigateTo<T>() where T : Window
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var window = _serviceProvider.GetRequiredService<T>();
            window.Show();
            currentWindow?.Close();
        }
        /*public void NavigateTo<T>() where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            window.Show();
            CloseCurrentWindow();
        }*/
        public void NavigateToMain()
        {
            // Replace MainWindow with your actual main window class
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            CloseCurrentWindow();
        }
        public void CloseCurrentWindow()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            currentWindow?.Close();
        }
        /*public void CloseCurrentWindow()
        {
            if (Application.Current.Windows.Count > 0)
            {
                var currentWindow = Application.Current.Windows[Application.Current.Windows.Count - 1];
                currentWindow.Close();
            }
        }*/
    }
}
