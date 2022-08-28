using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CustomerApp.Services;
using CustomerLib;

namespace CustomerApp.ViewModel
{
    public class WindowClosedMessage: ValueChangedMessage<CustomerViewModel>
    {
        public WindowClosedMessage(CustomerViewModel vm) : base(vm)
        {

        }
    }

    public class ViewModelDeletedMessage : ValueChangedMessage<CustomerViewModel>
    {
        public ViewModelDeletedMessage(CustomerViewModel vm) : base(vm)
        {

        }
    }

    public partial class MainViewModel : ObservableObject
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private ObservableCollection<CustomerViewModel> _openWindows = new ObservableCollection<CustomerViewModel>();
        
        public MainViewModel(ICustomerRepository customerRepository, 
            INavigationService navigationService,
            IMessenger messenger)
        {
            _customerRepository = customerRepository ?? 
                                  throw new ArgumentNullException("customerRepository");
            _navigationService = navigationService ?? 
                throw new ArgumentNullException("navigationService"); 
            _messenger = messenger ??
                throw new ArgumentNullException("messenger");
            Customers = new ObservableCollection<CustomerViewModel>(
                _customerRepository.Customers.Select(c => new CustomerViewModel(c, messenger)));
            messenger.Register<WindowClosedMessage>(this, (r, m) =>
            {
                WindowCount--;
                _openWindows.Remove(m.Value);
            });
            messenger.Register<ViewModelDeletedMessage>(this, (r, m) =>
            {
                DeleteCustomer(m.Value);
            });
        }

        private void DeleteCustomer(CustomerViewModel vm)
        {
            Customers.Remove(vm);
            var deletedCustomer = _customerRepository.Customers.FirstOrDefault(c => c.CustomerId == vm.CustomerId);
            if (deletedCustomer != null)
            {
                _customerRepository.Remove(deletedCustomer);
            }
            _navigationService.Close(vm);
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
            var vm = new CustomerViewModel(customer, _messenger);
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
            _openWindows.Add(vm);
        }
    }
}