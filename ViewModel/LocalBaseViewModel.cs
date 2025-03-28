using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection.Metadata;

namespace RentARideDB.ViewModel;

public partial class LocalBaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;
    private string _greeting;

    // Event for property change notifications


    // Greeting property
    public string Greeting
    {
        get => _greeting;
        set
        {
            if (_greeting != value)
            {
                _greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }
    }

    public bool IsNotBusy => !IsBusy;
}