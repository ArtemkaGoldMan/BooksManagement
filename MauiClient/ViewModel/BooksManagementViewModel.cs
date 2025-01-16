// BooksManagementViewModel.cs
using BaseLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;
using System.Collections.ObjectModel;
using BaseLibrary.Response;

namespace MauiClient.ViewModel;

public partial class BooksManagementViewModel : ObservableObject
{
    private readonly BooksService _booksService;
    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<BookDTO> books;

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

    public BooksManagementViewModel(BooksService booksService)
    {
        _booksService = booksService;
        Books = new ObservableCollection<BookDTO>();
        SelectedSortOption = "Title"; // Default sort option
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
                Books.Clear();
                foreach (var book in response.Items)
                {
                    Books.Add(book);
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
    private async Task AddBookAsync()
    {
        try
        {
            var newBook = new CreateBookDTO
            {
                Title = "",
                Author = "",
                Price = 0,
                PublishedDate = DateTime.Now,
                Genre = ""
            };

            // Prompt for Title
            string result = await Shell.Current.DisplayPromptAsync("Add New Book", "Enter book title:", initialValue: newBook.Title);
            if (result == null) return;
            newBook.Title = result;

            // Prompt for Author
            result = await Shell.Current.DisplayPromptAsync("Add New Book", "Enter author name:", initialValue: newBook.Author);
            if (result == null) return;
            newBook.Author = result;

            // Prompt for Price
            result = await Shell.Current.DisplayPromptAsync("Add New Book", "Enter book price:", initialValue: newBook.Price.ToString());
            if (result == null || !decimal.TryParse(result, out decimal price)) return;
            newBook.Price = price;

            // Prompt for Published Date
            result = await Shell.Current.DisplayPromptAsync("Add New Book", "Enter published date (yyyy-MM-dd):", initialValue: newBook.PublishedDate.ToString("yyyy-MM-dd"));
            if (result == null || !DateTime.TryParse(result, out DateTime publishedDate)) return;
            newBook.PublishedDate = publishedDate;

            // Prompt for Genre
            result = await Shell.Current.DisplayPromptAsync("Add New Book", "Enter book genre:", initialValue: newBook.Genre);
            if (result == null) return;
            newBook.Genre = result;

            // Add the book
            if (await _booksService.AddBookAsync(newBook))
            {
                await LoadBooksAsync();
                await Shell.Current.DisplayAlert("Success", "Book added successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }


    [RelayCommand]
    private async Task EditBookAsync(BookDTO book)
    {
        try
        {
            var updateBook = new UpdateBookDTO
            {
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                PublishedDate = book.PublishedDate,
                Genre = book.Genre
            };

            string result = await Shell.Current.DisplayPromptAsync("Edit Book",
                "Enter book title:",
                initialValue: updateBook.Title);

            if (result != null)
            {
                updateBook.Title = result;
                result = await Shell.Current.DisplayPromptAsync("Edit Book",
                    "Enter author name:",
                    initialValue: updateBook.Author);

                if (result != null)
                {
                    updateBook.Author = result;
                    if (await _booksService.UpdateBookAsync(book.Id, updateBook))
                    {
                        await LoadBooksAsync();
                        await Shell.Current.DisplayAlert("Success", "Book updated successfully!", "OK");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteBookAsync(BookDTO book)
    {
        try
        {
            bool answer = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {book.Title}?",
                "Yes",
                "No");

            if (answer)
            {
                bool success = await _booksService.DeleteBookAsync(book.Id);
                if (success)
                {
                    await LoadBooksAsync();
                    await Shell.Current.DisplayAlert("Success", "Book deleted successfully!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete book", "OK");
                }
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