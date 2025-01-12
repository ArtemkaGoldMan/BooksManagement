using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Services
{
    public interface IMyNavigationService
    {
        void NavigateTo<T>() where T : System.Windows.Window;
        void NavigateToMain();


        void CloseCurrentWindow();
    }
}
