using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFClient.Helpers;
using WPFClient.Services;
using WPFClient.Views;

namespace WPFClient.ViewModels
{
    public class EditBookViewModel : BaseViewModel
    {
        private readonly IBooksService _bookService;
        private readonly IMyNavigationService _navigationService;
        private readonly int _bookId;
        private string _title;
        private string _author;
        private decimal _price;
        private DateTime _publishedDate;
        private string _genre;
        private string _message;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public DateTime PublishedDate
        {
            get => _publishedDate;
            set => SetProperty(ref _publishedDate, value);
        }

        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditBookViewModel(IBooksService bookService, IMyNavigationService navigationService, BookDTO book)
        {
            _bookService = bookService;
            _navigationService = navigationService;
            _bookId = book.Id;

            // Initialize properties with existing book data
            Title = book.Title;
            Author = book.Author;
            Price = book.Price;
            PublishedDate = book.PublishedDate;
            Genre = book.Genre;

            SaveCommand = new RelayCommand(async () => await ExecuteSave(), CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteSave()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Author)
                && !string.IsNullOrWhiteSpace(Genre)
                && Price >= 0;
        }

        private async Task ExecuteSave()
        {
            try
            {
                var bookDto = new UpdateBookDTO
                {
                    Title = Title,
                    Author = Author,
                    Price = Price,
                    PublishedDate = PublishedDate,
                    Genre = Genre
                };

                var success = await _bookService.UpdateBookAsync(_bookId, bookDto);
                if (success)
                {
                    _navigationService.NavigateTo<AdminBooksWindow>();
                }
                else
                {
                    Message = "Failed to update book";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error updating book: {ex.Message}";
            }
        }

        private void ExecuteCancel()
        {
            _navigationService.NavigateTo<AdminBooksWindow>();
        }
    }
}
