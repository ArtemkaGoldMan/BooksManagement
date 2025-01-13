using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WPFClient.Helpers;
using WPFClient.Services;
using WPFClient.Views;

namespace WPFClient.ViewModels
{
    public class AdminBooksViewModel : BaseViewModel
    {
        private readonly IBooksService _bookService;
        private readonly IAuthenticationService _authService;
        private readonly IMyNavigationService _navigationService;
        private BookDTO _selectedBook;
        private string _searchTerm = "";
        private string _message;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        private string _sortColumn = "Title";
        private bool _ascending = true;

        public ObservableCollection<BookDTO> Books { get; private set; }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    _ = LoadBooks();
                }
            }
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public BookDTO SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }


        private bool _isAllBooksSelected = true;

        public bool IsAllBooksSelected
        {
            get => _isAllBooksSelected;
            set => SetProperty(ref _isAllBooksSelected, value);
        }

        public bool IsBorrowedBooksSelected
        {
            get => !_isAllBooksSelected;
            set => IsAllBooksSelected = !value;
        }

        public ICommand LogoutCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SortCommand { get; }

        public ICommand ShowBorrowedBooksCommand { get; }

        public AdminBooksViewModel(
            IBooksService bookService,
            IAuthenticationService authService,
            IMyNavigationService navigationService)
        {
            _bookService = bookService;
            _authService = authService;
            _navigationService = navigationService;

            Books = new ObservableCollection<BookDTO>();

            LogoutCommand = new RelayCommand(ExecuteLogout);
            EditBookCommand = new RelayCommand(ExecuteEditBook, () => SelectedBook != null);
            DeleteBookCommand = new RelayCommand(async () => await ExecuteDeleteBook(), () => SelectedBook != null);
            AddBookCommand = new RelayCommand(ExecuteAddBook);
            NextPageCommand = new RelayCommand(async () => await ExecuteNextPage(), CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(async () => await ExecutePreviousPage(), CanExecutePreviousPage);
            ClearSearchCommand = new RelayCommand(async () => await ExecuteClearSearch());
            SortCommand = new RelayCommand<string>(async (column) => await ExecuteSort(column));
            ShowBorrowedBooksCommand = new RelayCommand(() => _navigationService.NavigateTo<AdminBorrowedBooksWindow>());
        }

        public async Task Initialize()
        {
            await LoadBooks();
        }

        public async Task LoadBooks()
        {
            try
            {
                var response = await _bookService.GetBooksAsync(
                    SearchTerm,
                    _sortColumn,
                    _ascending,
                    _currentPage,
                    _pageSize);

                Books.Clear();
                foreach (var book in response.Items)
                {
                    Books.Add(book);
                }
                _totalPages = (int)Math.Ceiling((double)response.FilteredCount / _pageSize);
            }
            catch (Exception ex)
            {
                Message = $"Error loading books: {ex.Message}";
            }
        }

        private void ExecuteLogout()
        {
            _authService.Logout();
            _navigationService.NavigateTo<LoginView>();
        }

        private void ExecuteEditBook()
        {
            if (SelectedBook != null)
            {
                _navigationService.NavigateTo<EditBookView>(SelectedBook);
            }
        }

        private async Task ExecuteDeleteBook()
        {
            if (SelectedBook == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedBook.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _bookService.DeleteBookAsync(SelectedBook.Id);
                    if (success)
                    {
                        Message = "Book deleted successfully";
                        await LoadBooks();
                    }
                    else
                    {
                        Message = "Failed to delete book";
                    }
                }
                catch (Exception ex)
                {
                    Message = $"Error deleting book: {ex.Message}";
                }
            }
        }

        private void ExecuteAddBook()
        {
            _navigationService.NavigateTo<AddBookView>();
        }

        private bool CanExecuteNextPage() => _currentPage < _totalPages;

        private async Task ExecuteNextPage()
        {
            if (CanExecuteNextPage())
            {
                _currentPage++;
                await LoadBooks();
            }
        }

        private bool CanExecutePreviousPage() => _currentPage > 1;

        private async Task ExecutePreviousPage()
        {
            if (CanExecutePreviousPage())
            {
                _currentPage--;
                await LoadBooks();
            }
        }

        private async Task ExecuteClearSearch()
        {
            SearchTerm = string.Empty;
            await LoadBooks();
        }

        private async Task ExecuteSort(string column)
        {
            if (_sortColumn == column)
            {
                _ascending = !_ascending;
            }
            else
            {
                _sortColumn = column;
                _ascending = true;
            }
            await LoadBooks();
        }
    }
}
