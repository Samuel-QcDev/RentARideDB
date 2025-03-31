using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;
public partial class LoginPage : ContentPage
{
    private double LoginProgress { get; set; }
    private static ProgressBar LoginProgressBar;

    private LoginViewModel vm = new();
    private readonly ApplicationDbContext _dbContext;

    public LoginPage()
	{
        
        InitializeComponent();
        LoginProgressBar = new ProgressBar();
        BindingContext = vm;
        LoginStackLayout.Children.Add(LoginProgressBar);
    }
    // Temporary method for Submit button, will be changed to a command
    //private void Forgot_Clicked(object sender, EventArgs e)
    //{
    //    Navigation.PushAsync(new MainPage());
    //}
}