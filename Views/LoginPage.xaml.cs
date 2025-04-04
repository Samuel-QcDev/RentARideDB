using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;
public partial class LoginPage : ContentPage
{
    private double LoginProgress { get; set; }
    private static ProgressBar LoginProgressBar;

    //private LoginViewModel vm = new();
    public LoginPage(LoginViewModel vm)
	{
        InitializeComponent();
        LoginProgressBar = new ProgressBar();
        BindingContext = vm;
        LoginStackLayout.Children.Add(LoginProgressBar);
    }
}