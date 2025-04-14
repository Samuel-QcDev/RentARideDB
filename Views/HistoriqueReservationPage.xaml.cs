using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class HistoriqueReservationPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;

    public HistoriqueReservationPage()

    {
		InitializeComponent();
        var dbContext = new ApplicationDbContext();
        HistoriqueReservationViewModel vm = new HistoriqueReservationViewModel(_dbContext);
        BindingContext = vm;
	}

}