using Microsoft.Extensions.DependencyInjection;
using System;
using VirtualDebts.Binding;
using VirtualDebts.Controllers;
using VirtualDebts.Models;
using VirtualDebts.Services;
using VirtualDebts.UseCases;
using VirtualDebts.ViewModels;
using VirtualDebts.Views;
using Xamarin.Forms;
using IDispatcher = VirtualDebts.Binding.IDispatcher;

namespace VirtualDebts
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            InitializeComponent();

            this.serviceProvider = CreateServiceProvider();
            MainPage = new NavigationPage(this.serviceProvider.GetRequiredService<MainMenuView>());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EmptyView>();

            services.AddSingleton<ICommandFactory, CommandFactory>();
            services.AddSingleton<IDispatcher, XamarinDispatcher>();
            services.AddSingleton<XamarinViewProvider>();
            services.AddSingleton<INavigationService, XamarinNavigationService>();
            services.AddSingleton<Store<AppState>>();

            services.AddSingleton<MainMenuController>();
            services.AddSingleton<MainMenuView>(provider =>
                new MainMenuView { BindingContext = provider.GetRequiredService<MainMenuController>() }
            );

            services.AddScoped<IEditUsersInteractor, EditUsersInteractor>();
            services.AddScoped<EditUsersController>();
            services.AddScoped<EditUsersViewModel>();
            services.AddTransient<EditUsersView>(provider =>
                new EditUsersView { BindingContext = provider.GetRequiredService<EditUsersController>() }
            );

            services.AddSingleton<Server.IUserIdGenerator, Server.IdGenerator>();
        }
    }
}
