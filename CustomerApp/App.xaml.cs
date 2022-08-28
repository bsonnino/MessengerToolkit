using System;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using CustomerApp.Services;
using CustomerApp.ViewModel;
using CustomerLib;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Services = ConfigureServices();
        }

        public new static App Current => (App) Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<MainViewModel>();

            return services.BuildServiceProvider();
        }

        public MainViewModel MainVM => Services.GetService<MainViewModel>();
    }
}