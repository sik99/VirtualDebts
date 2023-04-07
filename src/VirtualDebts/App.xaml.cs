using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using VirtualDebts.Binding;
using VirtualDebts.Controllers;
using VirtualDebts.Models;
using VirtualDebts.Services;
using VirtualDebts.UseCases;
using VirtualDebts.Views;

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
            services.AddSingleton<IDispatcher, MauiDispatcher>();
            services.AddSingleton<MauiViewProvider>();
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
}
