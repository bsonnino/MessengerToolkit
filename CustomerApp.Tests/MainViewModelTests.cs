using System;
using System.Windows.Data;
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
            Action act = () => new MainViewModel(null, null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("customerRepository"));
        }

        [TestMethod]
        public void Constructor_Customers_ShouldHaveValue()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService);

            vm.Customers.Count.Should().Be(customers.Count);
        }

        [TestMethod]
        public void AddCommand_ShouldAddInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService);
            vm.AddCommand.Execute(null);
            A.CallTo(() => repository.Add(A<Customer>._)).MustHaveHappened();
        }

        [TestMethod]
        public void AddCommand_ShouldAddInCollection()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService);
            vm.AddCommand.Execute(null);
            vm.Customers.Count.Should().Be(11);
        }

        [TestMethod]
        public void SaveCommand_ShouldCommitInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var vm = new MainViewModel(repository, navigationService);
            vm.SaveCommand.Execute(null);
            A.CallTo(() => repository.Commit()).MustHaveHappened();
        }

        [TestMethod]
        public void SearchCommand_WithText_ShouldSetFilter()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService);
            vm.SearchCommand.Execute("text");
            var coll = CollectionViewSource.GetDefaultView(vm.Customers);
            coll.Filter.Should().NotBeNull();
        }

        [TestMethod]
        public void SearchCommand_WithoutText_ShouldSetFilter()
        {
            var repository = A.Fake<ICustomerRepository>();
            var navigationService = A.Fake<INavigationService>();
            var customers = A.CollectionOfFake<Customer>(10);
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository, navigationService);
            vm.SearchCommand.Execute("");
            var coll = CollectionViewSource.GetDefaultView(vm.Customers);
            coll.Filter.Should().BeNull();
        }
    }
}
