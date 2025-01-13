using MauiClient.ViewModel;

namespace MauiClient.Views;

public partial class BorrowHistoryPage : ContentPage
{
    public BorrowHistoryPage(BorrowHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Load initial data
        _ = viewModel.LoadAllHistoryAsync();
    }
}