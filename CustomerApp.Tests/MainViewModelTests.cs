using System;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using CustomerApp.Services;
using CustomerApp.ViewModel;
using CustomerLib;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerApp.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void Constructor_NullRepository_ShouldThrow()
        {
            Action act = () => new MainViewModel(null, null, null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("customerRepository"));
        }

        [TestMethod]
        public void Constructor_NullNavigationService_ShouldThrow()
        {
            Action act = () => new MainViewModel(A.Fake<ICustomerRepository>(), null, null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("navigationService"));
        }

        [TestMethod]
        public void Constructor_NullMessenger_ShouldThrow()
        {
            Action act = () => new MainViewModel(A.Fake<ICustomerRepository>(),
                A.Fake<INavigationService>(), null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("messenger"));
        }

        [TestMethod]
        public void Constructor_Customers_ShouldHaveValue()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);

            vm.Customers.Count.Should().Be(customers.Count);
        }

        [TestMethod]
        public void AddCommand_ShouldAddInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.AddCommand.Execute(null);
            A.CallTo(() => repository.Add(A<Customer>._)).MustHaveHappened();
        }

        [TestMethod]
        public void AddCommand_ShouldAddInCollection()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.AddCommand.Execute(null);
            vm.Customers.Count.Should().Be(11);
        }

        [TestMethod]
        public void AddCommand_ShouldCallNavigate()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.AddCommand.Execute(null);
            A.CallTo(() => navigationService.Navigate(A<CustomerViewModel>.Ignored)).MustHaveHappened();
        }

        [TestMethod]
        public void SaveCommand_ShouldCommitInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.SaveCommand.Execute(null);
            A.CallTo(() => repository.Commit()).MustHaveHappened();
        }

        [TestMethod]
        public void SearchCommand_WithText_ShouldSetFilter()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.SearchCommand.Execute("text");
            var coll = CollectionViewSource.GetDefaultView(vm.Customers);
            coll.Filter.Should().NotBeNull();
        }

        [TestMethod]
        public void SearchCommand_WithoutText_ShouldSetFilter()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            vm.SearchCommand.Execute("");
            var coll = CollectionViewSource.GetDefaultView(vm.Customers);
            coll.Filter.Should().BeNull();
        }

        [TestMethod]
        public void ShowDetailsCommand_ShouldCallNavigate()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            A.CallTo(() => navigationService.Navigate(customerVM)).MustHaveHappened();
        }

        [TestMethod]
        public void ShowDetailsCommand_ShouldIncrementWindowCount()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            vm.WindowCount.Should().Be(1);
        }

        [TestMethod]
        public void ShowDetailsCommand_ShouldAddToOpenWindows()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = A.Fake<IMessenger>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            vm.OpenWindows.Count.Should().Be(1);
            vm.OpenWindows[0].Should().Be(customerVM);
        }

        [TestMethod]
        public void CustomerCloseCommand_ShouldDecreaseWindowCount()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = WeakReferenceMessenger.Default;
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            customerVM.ClosingCommand.Execute(null);
            vm.WindowCount.Should().Be(0);
        }

        [TestMethod]
        public void CustomerCloseCommand_ShouldRemoveFromOpenWindows()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = WeakReferenceMessenger.Default;
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            customerVM.ClosingCommand.Execute(null);
            vm.OpenWindows.Count.Should().Be(0);
        }

        [TestMethod]
        public void CustomerDeleteCommand_ShouldCallNavigationClose()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = WeakReferenceMessenger.Default;
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            customerVM.DeleteCommand.Execute(null);
            A.CallTo(() => navigationService.Close(customerVM)).MustHaveHappened();
        }

        [TestMethod]
        public void CustomerDeleteCommand_ShouldRemoveCustomer()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = WeakReferenceMessenger.Default;
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            customerVM.DeleteCommand.Execute(null);
            vm.Customers.Count.Should().Be(9);
            vm.Customers.Should().NotContain(customerVM);
        }

        [TestMethod]
        public void CustomerDeleteCommand_ShouldCallRemoveFromRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var messenger = WeakReferenceMessenger.Default;
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService, messenger);
            CustomerViewModel customerVM = vm.Customers[1];
            vm.ShowDetailsCommand.Execute(customerVM);
            customerVM.DeleteCommand.Execute(null);
            A.CallTo(() => repository.Remove(A<Customer>.Ignored)).MustHaveHappened();
        }
    }
}
