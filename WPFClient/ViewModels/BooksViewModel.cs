using BaseLibrary.DTOs;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Windows.Input;
using WPFClient.Helpers;
using WPFClient.Services;
using WPFClient.ViewModels;
using WPFClient.Views;

public class BooksViewModel : BaseViewModel
{
    private readonly IBooksService _bookService;
    private readonly IAuthenticationService _authService;
    private readonly IMyNavigationService _navigationService;
    private int _loggedInUserId;
    private string _searchTerm = "";
    private string _sortColumn = "Title";
    private bool _ascending = true;
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages;
    private bool _showOnlyBorrowed;
    private string _message;
    private BookDTO _selectedBook;

    public ObservableCollection<BookDTO> Books { get; private set; }
    public ObservableCollection<BorrowDTO> BorrowedBooks { get; private set; }
    public ObservableCollection<BorrowDTO> AllBorrowedBooks { get; private set; }

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

    public bool ShowOnlyBorrowed
    {
        get => _showOnlyBorrowed;
        set
        {
            if (SetProperty(ref _showOnlyBorrowed, value))
            {
                _ = LoadBooks();
            }
        }
    }

    // Commands
    public ICommand LogoutCommand { get; }
    public ICommand BorrowCommand { get; }
    public ICommand ReturnCommand { get; }
    public ICommand ShowBorrowedBooksCommand { get; }
    public ICommand ShowAllBooksCommand { get; }
    public ICommand BackToAllBooksCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand SortCommand { get; private set; }

    public BooksViewModel(
        IBooksService bookService,
        IAuthenticationService authService,
        IMyNavigationService navigationService)
    {
        _bookService = bookService;
        _authService = authService;
        _navigationService = navigationService;

        Books = new ObservableCollection<BookDTO>();
        BorrowedBooks = new ObservableCollection<BorrowDTO>();
        AllBorrowedBooks = new ObservableCollection<BorrowDTO>();

        // Initialize commands
        LogoutCommand = new RelayCommand(ExecuteLogout);
        BorrowCommand = new RelayCommand(async () => await ExecuteBorrow(), CanExecuteBorrow);
        ReturnCommand = new RelayCommand(async () => await ExecuteReturn(), CanExecuteReturn);
        ShowBorrowedBooksCommand = new RelayCommand(() => ShowOnlyBorrowed = true);
        ShowAllBooksCommand = new RelayCommand(() => ShowOnlyBorrowed = false);
        BackToAllBooksCommand = new RelayCommand(ExecuteBackToAllBooks);
        NextPageCommand = new RelayCommand(async () => await ExecuteNextPage(), CanExecuteNextPage);
        PreviousPageCommand = new RelayCommand(async () => await ExecutePreviousPage(), CanExecutePreviousPage);
        ClearSearchCommand = new RelayCommand(async () => await ExecuteClearSearch());
        SortCommand = new RelayCommand<string>(async (column) => await ExecuteSort(column));
    }

    public async Task Initialize()
    {
        await SetLoggedInUserId();
        await LoadBooks();
        await LoadBorrowedBooks();
    }

    public async Task LoadBooks()
    {
        try
        {
            if (ShowOnlyBorrowed)
            {
                Books.Clear();
                var borrowedBooks = BorrowedBooks.Where(b => b.UserId == _loggedInUserId);
                foreach (var borrowed in borrowedBooks)
                {
                    Books.Add(new BookDTO
                    {
                        Id = borrowed.BookId,
                        Title = borrowed.BookTitle,
                        Author = borrowed.UserName,
                        Genre = "Borrowed",
                        // Add other properties as needed
                    });
                }
                _totalPages = 1;
            }
            else
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
        }
        catch (Exception ex)
        {
            Message = $"Error loading books: {ex.Message}";
        }
    }

    public async Task LoadBorrowedBooks()
    {
        try
        {
            var userBorrowedBooks = await _bookService.GetBorrowedBooksByUserAsync(_loggedInUserId);
            BorrowedBooks.Clear();
            foreach (var book in userBorrowedBooks ?? Enumerable.Empty<BorrowDTO>())
            {
                BorrowedBooks.Add(book);
            }

            var allBorrowed = await _bookService.GetAllBorrowedBooksAsync();
            AllBorrowedBooks.Clear();
            foreach (var book in allBorrowed ?? Enumerable.Empty<BorrowDTO>())
            {
                AllBorrowedBooks.Add(book);
            }
        }
        catch (Exception ex)
        {
            Message = $"Error loading borrowed books: {ex.Message}";
        }
    }

    private bool CanExecuteBorrow()
    {
        if (SelectedBook == null) return false;
        return !AllBorrowedBooks.Any(b => b.BookId == SelectedBook.Id);
    }

    private async Task ExecuteBorrow()
    {
        if (SelectedBook == null) return;

        try
        {
            var request = new BorrowRequestDTO
            {
                BookId = SelectedBook.Id,
                UserId = _loggedInUserId
            };

            var success = await _bookService.BorrowBookAsync(request);
            if (success)
            {
                Message = "Book borrowed successfully!";
                await LoadBorrowedBooks();
                await LoadBooks();
            }
            else
            {
                Message = "Failed to borrow the book. It may already be borrowed.";
            }
        }
        catch (Exception ex)
        {
            Message = $"Error borrowing book: {ex.Message}";
        }
    }

    private bool CanExecuteReturn()
    {
        if (SelectedBook == null) return false;
        return BorrowedBooks.Any(b => b.BookId == SelectedBook.Id && b.UserId == _loggedInUserId);
    }

    private async Task ExecuteReturn()
    {
        if (SelectedBook == null) return;

        try
        {
            var success = await _bookService.ReturnBookAsync(SelectedBook.Id, _loggedInUserId);
            if (success)
            {
                Message = "Book returned successfully!";
                await LoadBorrowedBooks();
                await LoadBooks();
            }
            else
            {
                Message = "Failed to return the book.";
            }
        }
        catch (Exception ex)
        {
            Message = $"Error returning book: {ex.Message}";
        }
    }

    private void ExecuteLogout()
    {
        _authService.Logout();
        _navigationService.NavigateTo<LoginView>();
    }

    private void ExecuteBackToAllBooks()
    {
        ShowOnlyBorrowed = false;
        _navigationService.NavigateTo<BooksWindow>();
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

    private async Task SetLoggedInUserId()
    {
        var claimsPrincipal = _authService.GetAuthenticationState();
        //var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        var userIdClaim = claimsPrincipal.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            _loggedInUserId = userId;
        }
        else
        {
            Message = "Failed to retrieve UserId.";
        }
    }
}