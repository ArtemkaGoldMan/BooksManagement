namespace MauiClient.Views;
using MauiClient.ViewModel;

public partial class AllBooksMember : ContentPage
{
    public AllBooksMember(AllBooksMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        Resources.Add("BoolToStatusConverter", new BoolToStatusConverter());
        Resources.Add("BoolToColorConverter", new BoolToColorConverter());
        Resources.Add("BoolToButtonColorConverter", new BoolToButtonColorConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllBooksMemberViewModel viewModel)
        {
            await viewModel.LoadBooksAsync();
        }
    }
}

public class BoolToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? "Available" : "Borrowed";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? Colors.Green : Colors.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToButtonColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? Colors.DodgerBlue : Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}