using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomerLib;

namespace CustomerApp.ViewModel
{
    public partial class CustomerViewModel : ObservableObject
    {
        private readonly Customer _customer;

        public CustomerViewModel(Customer customer)
        {
            _customer = customer;
            CustomerId = _customer.CustomerId;
            CompanyName = _customer.CompanyName;
            ContactName = _customer.ContactName;
            ContactTitle = _customer.ContactTitle;
            Address = _customer.Address;
            City = _customer.City;
            Region = _customer.Region;
            PostalCode = _customer.PostalCode;
            Country = _customer.Country;
            Phone = _customer.Phone;
            Fax = _customer.Fax;
        }

        [ObservableProperty] 
        private string _customerId;
        [ObservableProperty] 
        private string _companyName;
        [ObservableProperty] 
        private string _contactName;
        [ObservableProperty] 
        private string _contactTitle;
        [ObservableProperty] 
        private string _address;
        [ObservableProperty] 
        private string _city;
        [ObservableProperty] 
        private string _region;
        [ObservableProperty] 
        private string _postalCode;
        [ObservableProperty] 
        private string _country;
        [ObservableProperty] 
        private string _phone;
        [ObservableProperty] 
        private string _fax;

        [RelayCommand]
        private void Delete()
        {

        }

        [RelayCommand]
        private void Closing()
        {

        }
    }
}
