using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using VirtualDebts.Binding;
using VirtualDebts.Controllers;
using VirtualDebts.Models;
using VirtualDebts.Services;
using VirtualDebts.UseCases;
using VirtualDebts.Views;

using IDispatcher = VirtualDebts.Binding.IDispatcher;

namespace VirtualDebts;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

        ConfigureServices(builder.Services);

		return builder.Build();
	}

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EmptyView>();

        services.AddSingleton<ICommandFactory, CommandFactory>();
        services.AddSingleton<IDispatcher, MauiDispatcher>();
        services.AddSingleton<INavigationService, MauiNavigationService>();
        services.AddSingleton<Store<AppState>>();

        services.AddSingleton<MainMenuView>();
        services.AddSingleton<MainMenuController>();

        services.AddTransient<EditUsersView>();
        services.AddTransient<EditUsersController>();
        services.AddTransient<IEditUsersInteractor, EditUsersInteractor>();
        services.AddSingleton<Server.IUserIdGenerator, Server.IdGenerator>();
    }
}
