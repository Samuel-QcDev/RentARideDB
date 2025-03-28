using Microsoft.Maui.Controls;
using RentARideDB.Views;
using RentARideDB.ViewModel;
using RentARideDB.Models;

namespace RentARideDB.Controls;

public partial class HeaderView : ContentView
{
    //private ReservationSearchViewModel vm = new ();
    public HeaderView(ReservationSearchViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
        Console.WriteLine($"BindingContext: {BindingContext}");

    }
    // Bindable property to set the greeting message dynamically
    public static readonly BindableProperty GreetingProperty =
        BindableProperty.Create(nameof(Greeting), typeof(string), typeof(HeaderView), string.Empty);

    public string Greeting
    {
        get => (string)GetValue(GreetingProperty);
        set => SetValue(GreetingProperty, value);
    }
}