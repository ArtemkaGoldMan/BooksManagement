using MauiClient.Services;
using MauiClient.Views;

namespace MauiClient;

public partial class AppShell : Shell
{
    private readonly LoginService _loginService;
    private readonly ToolbarItem _logoutToolbarItem;

    public static readonly BindableProperty IsAuthenticatedProperty =
        BindableProperty.Create(nameof(IsAuthenticated), typeof(bool), typeof(AppShell), false, propertyChanged: OnIsAuthenticatedChanged);

    public static readonly BindableProperty UserRoleProperty =
        BindableProperty.Create(nameof(UserRole), typeof(string), typeof(AppShell), string.Empty, propertyChanged: OnUserRoleChanged);

    public bool IsAuthenticated
    {
        get => (bool)GetValue(IsAuthenticatedProperty);
        set => SetValue(IsAuthenticatedProperty, value);
    }

    public string UserRole
    {
        get => (string)GetValue(UserRoleProperty);
        set => SetValue(UserRoleProperty, value);
    }

    public AppShell()
    {
        InitializeComponent();

        // Retrieve the LoginService from the DI container
        _loginService = App.Services.GetService<LoginService>();

        // Register routes
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(BorrowHistoryPage), typeof(BorrowHistoryPage));
        Routing.RegisterRoute(nameof(BooksManagementPage), typeof(BooksManagementPage));

        // Create the Logout button
        _logoutToolbarItem = new ToolbarItem
        {
            Text = "Logout",
            Command = new Command(async () => await HandleLogoutAsync())
        };

        // Initialize the authentication state and user role
        InitializeAuthenticationState();
    }

    private async void InitializeAuthenticationState()
    {
        IsAuthenticated = !string.IsNullOrEmpty(await SecureStorage.GetAsync("JwtToken"));
        UserRole = await SecureStorage.GetAsync("UserRole") ?? string.Empty;

        Console.WriteLine($"Initialized UserRole: {UserRole}");
        Console.WriteLine($"Initialized IsAuthenticated: {IsAuthenticated}");

        UpdateToolbarAndTabs();
    }

    private async Task HandleLogoutAsync()
    {
        await _loginService.LogoutAsync();
        SecureStorage.Default.Remove("JwtToken");
        SecureStorage.Default.Remove("UserRole");

        // Clear authentication state
        IsAuthenticated = false;
        UserRole = string.Empty;

        // Navigate to the Login Page
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private void UpdateToolbarAndTabs()
    {
        if (IsAuthenticated)
        {
            MainTabBar.IsVisible = true; // Show TabBar
            if (!ToolbarItems.Contains(_logoutToolbarItem))
            {
                ToolbarItems.Add(_logoutToolbarItem); // Add Logout button
            }

            // Show or hide the BorrowHistoryPage tab based on the user role
            var borrowHistoryTab = MainTabBar.Items.FirstOrDefault(t => t.Title == "Borrow History");
            if (borrowHistoryTab != null)
            {
                // Use case-insensitive comparison
                borrowHistoryTab.IsVisible = UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }


            var booksManagementTab = MainTabBar.Items.FirstOrDefault(t => t.Title == "Books Management");
            if (booksManagementTab != null)
            {
                booksManagementTab.IsVisible = UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }


            var allBooksTab = MainTabBar.Items.FirstOrDefault(t => t.Title == "All Books");
            if (allBooksTab != null)
            {
                allBooksTab.IsVisible = UserRole.Equals("Member", StringComparison.OrdinalIgnoreCase);
            }

            var myBorrowedBooksTab = MainTabBar.Items.FirstOrDefault(t => t.Title == "My Borrowed Books");
            if (myBorrowedBooksTab != null)
            {
                myBorrowedBooksTab.IsVisible = UserRole.Equals("Member", StringComparison.OrdinalIgnoreCase);
            }

        }
        else
        {
            MainTabBar.IsVisible = false; // Hide TabBar
            if (ToolbarItems.Contains(_logoutToolbarItem))
            {
                ToolbarItems.Remove(_logoutToolbarItem); // Remove Logout button
            }
        }
    }

    private static void OnIsAuthenticatedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AppShell shell)
        {
            shell.UpdateToolbarAndTabs();
        }
    }

    private static void OnUserRoleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AppShell shell)
        {
            shell.UpdateToolbarAndTabs();
        }
    }
}
