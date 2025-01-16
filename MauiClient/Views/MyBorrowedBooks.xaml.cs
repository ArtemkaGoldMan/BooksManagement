using MauiClient.ViewModel;

namespace MauiClient.Views;


public partial class MyBorrowedBooks : ContentPage
{
    public MyBorrowedBooks(MyBorrowedBooksViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyBorrowedBooksViewModel viewModel)
        {
            await viewModel.LoadBorrowedBooksAsync();
        }
    }
}