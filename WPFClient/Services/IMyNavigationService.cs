using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient.Services
{
    public interface IMyNavigationService
    {
        void NavigateTo<T>(object? parameter = null) where T : Window;
        void NavigateToMain();


        void CloseCurrentWindow();
    }
}
