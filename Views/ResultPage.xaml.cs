using RentARideDB.ViewModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Runtime.Intrinsics.X86;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RentARideDB.Views;

public partial class ResultPage : ContentPage
{
    ReservationResultViewModel vm = new ReservationResultViewModel();
    public ResultPage()
	{
        BindingContext = vm;
        InitializeComponent();
	}
    //private void Submit_Clicked(object sender, EventArgs e)
    //{
    //    Navigation.PushAsync(new MainPage());
    //}
}