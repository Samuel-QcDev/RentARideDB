using RentARideDB.Views;

namespace RentARideDB;

public partial class AppShell : Shell
{
	public AppShell()
	{
        InitializeComponent();

        Routing.RegisterRoute("Loginpage", typeof(LoginPage));
        Routing.RegisterRoute("Mainpage", typeof(MainPage));
        Routing.RegisterRoute("Reservationpage", typeof(ReservationSearchPage));
        Routing.RegisterRoute("Historiquereservationpage", typeof(HistoriqueReservationPage));
        Routing.RegisterRoute("Resultpage", typeof(ResultPage));
        Routing.RegisterRoute("MembreDetailspage", typeof(MembreDetails));

    }
}
