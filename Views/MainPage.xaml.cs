using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
    }
}

