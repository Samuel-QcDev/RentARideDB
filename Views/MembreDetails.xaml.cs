using RentARideDB.ViewModel;

namespace RentARideDB.Views;

public partial class MembreDetails : ContentPage
{
    private MembreViewModel vm;
    private int id;
    private string name;
    private string level;

    public MembreDetails(MembreViewModel vm)
    {
        this.BindingContext = vm;
        InitializeComponent();
        
	}

    public MembreDetails(int id, string name, string level)
    {
        this.BindingContext = vm;
        InitializeComponent();
        this.id = id;
        this.name = name;
        this.level = level;
    }

    public object GetValue(MembreDetails membreDetails)
    {
        return $"{membreDetails.id}, {membreDetails.name}, {membreDetails.level}";
    }
}