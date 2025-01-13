using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFClient.Helpers;
using WPFClient.Services;
using WPFClient.Views;

namespace WPFClient.ViewModels
{
    public class AdminBorrowedBooksViewModel : BaseViewModel
    {
        private readonly IBooksService _bookService;
        private readonly IAuthenticationService _authService;
        private readonly IMyNavigationService _navigationService;
        private BorrowHistoryDTO _selectedBorrow;
        private string _searchTerm = "";
        private string _message;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        private bool _isAllBooksSelected;

        public ObservableCollection<BorrowHistoryDTO> BorrowedBooks { get; private set; }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    _ = LoadBorrowedBooks();
                }
            }
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public BorrowHistoryDTO SelectedBorrow
        {
            get => _selectedBorrow;
            set => SetProperty(ref _selectedBorrow, value);
        }

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
        public ICommand ShowAllBooksCommand { get; }
        public ICommand ShowBorrowedBooksCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public AdminBorrowedBooksViewModel(
            IBooksService bookService,
            IAuthenticationService authService,
            IMyNavigationService navigationService)
        {
            _bookService = bookService;
            _authService = authService;
            _navigationService = navigationService;

            BorrowedBooks = new ObservableCollection<BorrowHistoryDTO>();

            LogoutCommand = new RelayCommand(ExecuteLogout);
            ShowAllBooksCommand = new RelayCommand(() => _navigationService.NavigateTo<AdminBooksWindow>());
            ShowBorrowedBooksCommand = new RelayCommand(() => _navigationService.NavigateTo<AdminBorrowedBooksWindow>());
            NextPageCommand = new RelayCommand(async () => await ExecuteNextPage(), CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(async () => await ExecutePreviousPage(), CanExecutePreviousPage);
            ClearSearchCommand = new RelayCommand(async () => await ExecuteClearSearch());

            IsBorrowedBooksSelected = true;
        }

        public async Task Initialize()
        {
            await LoadBorrowedBooks();
        }

        private async Task LoadBorrowedBooks()
        {
            try
            {
                var borrowHistory = await _bookService.GetBorrowHistoryAsync();
                BorrowedBooks.Clear();

                var filteredHistory = borrowHistory
                    .Where(b => string.IsNullOrEmpty(SearchTerm) ||
                               b.BookTitle.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                               b.UserName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

                foreach (var borrow in filteredHistory)
                {
                    BorrowedBooks.Add(borrow);
                }
            }
            catch (Exception ex)
            {
                Message = $"Error loading borrowed books: {ex.Message}";
            }
        }

        private void ExecuteLogout()
        {
            _authService.Logout();
            _navigationService.NavigateTo<LoginView>();
        }

        private bool CanExecuteNextPage() => _currentPage < _totalPages;

        private async Task ExecuteNextPage()
        {
            if (CanExecuteNextPage())
            {
                _currentPage++;
                await LoadBorrowedBooks();
            }
        }

        private bool CanExecutePreviousPage() => _currentPage > 1;

        private async Task ExecutePreviousPage()
        {
            if (CanExecutePreviousPage())
            {
                _currentPage--;
                await LoadBorrowedBooks();
            }
        }

        private async Task ExecuteClearSearch()
        {
            SearchTerm = string.Empty;
            await LoadBorrowedBooks();
        }
    }

}




