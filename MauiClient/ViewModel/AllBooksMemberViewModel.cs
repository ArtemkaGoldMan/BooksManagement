using BaseLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;
using System.Collections.ObjectModel;
using BaseLibrary.Response;

namespace MauiClient.ViewModel;

public partial class AllBooksMemberViewModel : ObservableObject
{
    private readonly BooksService _booksService;
    private readonly TokenService _tokenService;
    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<BookWithStatusDTO> books;

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private string selectedSortOption;

    [ObservableProperty]
    private bool isAscending = true;

    [ObservableProperty]
    private string sortOrderText = "Ascending ↑";

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private bool canGoPrevious;

    [ObservableProperty]
    private bool canGoNext;

    [ObservableProperty]
    private int totalBooks;

    public AllBooksMemberViewModel(BooksService booksService, TokenService tokenService)
    {
        _booksService = booksService;
        _tokenService = tokenService;
        Books = new ObservableCollection<BookWithStatusDTO>();
        SelectedSortOption = "Title";
    }

    [RelayCommand]
    public async Task LoadBooksAsync()
    {
        try
        {
            var response = await _booksService.GetBooksAsync(
                SearchText,
                SelectedSortOption,
                IsAscending,
                CurrentPage,
                PageSize);

            if (response != null)
            {
                var borrowedBooks = await _booksService.GetAllBorrowedBooksAsync();
                Books.Clear();

                foreach (var book in response.Items)
                {
                    var bookWithStatus = new BookWithStatusDTO
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        Price = book.Price,
                        PublishedDate = book.PublishedDate,
                        Genre = book.Genre,
                        IsAvailable = !borrowedBooks.Any(b => b.BookId == book.Id)
                    };
                    Books.Add(bookWithStatus);
                }

                TotalBooks = response.TotalCount;
                CanGoPrevious = CurrentPage > 1;
                CanGoNext = (CurrentPage * PageSize) < response.TotalCount;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load books: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadBooksAsync();
    }

    [RelayCommand]
    private async Task BorrowBookAsync(BookWithStatusDTO book)
    {
        if (!book.IsAvailable)
        {
            await Shell.Current.DisplayAlert("Error", "This book is not available for borrowing.", "OK");
            return;
        }

        try
        {
            var userId = await _tokenService.GetLoggedInUserIdAsync();
            if (!userId.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "User not found. Please log in again.", "OK");
                return;
            }

            var request = new BorrowRequestDTO
            {
                BookId = book.Id,
                UserId = userId.Value
            };

            var result = await _booksService.BorrowBookAsync(request);
            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success", $"Successfully borrowed: {book.Title}", "OK");
                await LoadBooksAsync(); // Refresh the list
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task ToggleSortOrderAsync()
    {
        IsAscending = !IsAscending;
        SortOrderText = IsAscending ? "Ascending ↑" : "Descending ↓";
        await LoadBooksAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadBooksAsync();
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadBooksAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        SearchCommand.Execute(null);
    }

    partial void OnSelectedSortOptionChanged(string value)
    {
        LoadBooksCommand.Execute(null);
    }
}

public class BookWithStatusDTO : BookDTO
{
    public bool IsAvailable { get; set; }
}