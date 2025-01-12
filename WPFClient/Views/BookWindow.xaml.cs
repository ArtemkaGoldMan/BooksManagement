using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient.Services;
using WPFClient.ViewModels;


namespace WPFClient.Views
{
    public partial class BooksWindow : Window
    {
        private readonly BooksViewModel _viewModel;

        public BooksWindow(BooksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += BooksWindow_Loaded;
            Closed += BooksWindow_Closed;
        }

        private async void BooksWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.Initialize();
            // LoadBooks is now called within Initialize
        }

        private void BooksWindow_Closed(object sender, EventArgs e)
        {
            // Clean up when window is closed
            Loaded -= BooksWindow_Loaded;
            Closed -= BooksWindow_Closed;
        }
    }
}
