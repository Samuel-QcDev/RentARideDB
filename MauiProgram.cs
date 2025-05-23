﻿using CommunityToolkit.Maui;
using RentARideDB.ViewModel;
using RentARideDB.Views;
using RentARideDB.Models;
using RentARideDB.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RentARideDB;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        // Services
        builder.Services.AddSingleton<ApplicationDbContext>();
        //builder.Services.AddSingleton<ReservationService>();


        // Views
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<HistoriqueReservationPage>();
        builder.Services.AddSingleton<ReservationSearchPage>();
        builder.Services.AddSingleton<MembreDetails>();

        // View Models
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<MainViewModel>(); 
        builder.Services.AddSingleton<HistoriqueReservationViewModel>();
        builder.Services.AddSingleton<ReservationSearchViewModel>();
        builder.Services.AddSingleton<MembreViewModel>();

        return builder.Build();
	}
}
