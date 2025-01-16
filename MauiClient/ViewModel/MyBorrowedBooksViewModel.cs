using BaseLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;
using System.Collections.ObjectModel;
using BaseLibrary.Response;

namespace MauiClient.ViewModel;

public partial class MyBorrowedBooksViewModel : ObservableObject
{
    private readonly BooksService _booksService;
    private readonly TokenService _tokenService;

    [ObservableProperty]
    private ObservableCollection<BorrowDTO> borrowedBooks;

    public MyBorrowedBooksViewModel(BooksService booksService, TokenService tokenService)
    {
        _booksService = booksService;
        _tokenService = tokenService;
        BorrowedBooks = new ObservableCollection<BorrowDTO>();
    }

    [RelayCommand]
    public async Task LoadBorrowedBooksAsync()
    {
        try
        {
            var userId = await _tokenService.GetLoggedInUserIdAsync();
            if (!userId.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "User not found. Please log in again.", "OK");
                return;
            }

            var books = await _booksService.GetBorrowedBooksByUserAsync(userId.Value);

            BorrowedBooks.Clear();
            foreach (var book in books)
            {
                BorrowedBooks.Add(book);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load borrowed books: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ReturnBookAsync(BorrowDTO borrow)
    {
        try
        {
            var userId = await _tokenService.GetLoggedInUserIdAsync();
            if (!userId.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "User not found. Please log in again.", "OK");
                return;
            }

            bool result = await _booksService.ReturnBookAsync(borrow.BookId, userId.Value);
            if (result)
            {
                await Shell.Current.DisplayAlert("Success", $"Successfully returned: {borrow.BookTitle}", "OK");
                await LoadBorrowedBooksAsync(); // Refresh the list
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to return the book", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}