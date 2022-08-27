using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomerApp.Services;
using CustomerLib;

namespace CustomerApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INavigationService _navigationService;
        
        public MainViewModel(ICustomerRepository customerRepository, INavigationService navigationService)
        {
            _customerRepository = customerRepository ?? 
                                  throw new ArgumentNullException("customerRepository");
            Customers = new ObservableCollection<CustomerViewModel>(
                _customerRepository.Customers.Select(c => new CustomerViewModel(c)));
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private ObservableCollection<CustomerViewModel> _customers;

        [ObservableProperty]
        private int _windowCount;

        [RelayCommand]
        private void Add()
        {
            var customer = new Customer();
            _customerRepository.Add(customer);
            var vm = new CustomerViewModel(customer);
            Customers.Add(vm);
            _navigationService.Navigate(vm);
        }

        [RelayCommand]
        private void Save()
        {
            _customerRepository.Commit();
        }

        [RelayCommand]
        private void Search(string textToSearch)
        {
            var coll = CollectionViewSource.GetDefaultView(Customers);
            if (!string.IsNullOrWhiteSpace(textToSearch))
                coll.Filter = c => ((CustomerViewModel)c).Country?.ToLower().Contains(textToSearch.ToLower()) == true;
            else
                coll.Filter = null;
        }

        [RelayCommand]
        private void ShowDetails(CustomerViewModel vm)
        {
            _navigationService.Navigate(vm);
            WindowCount++;
        }
    }
}