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
using WPFClient.ViewModels;
namespace WPFClient.Views
{
    /// <summary>
    /// Interaction logic for BorrowedBooksWindow.xaml
    /// </summary>
    public partial class BorrowedBooksWindow : Window
    {
        private readonly BooksViewModel _viewModel;

        public BorrowedBooksWindow(BooksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += BorrowedBooksWindow_Loaded;
            Closed += BorrowedBooksWindow_Closed;
        }

        private async void BorrowedBooksWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Make sure we show only borrowed books when this window is loaded
            _viewModel.ShowOnlyBorrowed = true;
            await _viewModel.LoadBorrowedBooks();
        }

        private void BorrowedBooksWindow_Closed(object sender, EventArgs e)
        {
            Loaded -= BorrowedBooksWindow_Loaded;
            Closed -= BorrowedBooksWindow_Closed;
        }
    }
}