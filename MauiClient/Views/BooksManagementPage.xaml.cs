// BooksManagementPage.xaml.cs
using MauiClient.ViewModel;

namespace MauiClient.Views;

public partial class BooksManagementPage : ContentPage
{
    public BooksManagementPage(BooksManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BooksManagementViewModel viewModel)
        {
            await viewModel.LoadBooksAsync();
        }
    }
}