using AutoBogus;
using CommunityToolkit.Mvvm.Messaging;
using CustomerApp.ViewModel;
using CustomerLib;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace CustomerApp.Tests
{
    [TestClass]
    public class CustomerViewModelTests
    {
        [TestMethod]
        public void Constructor_NullCustomer_ShouldThrow()
        {
            Action act = () => new CustomerViewModel(null,null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("customer"));
        }

        [TestMethod]
        public void Constructor_ShouldSetFields()
        {
            var customer = AutoFaker.Generate<Customer>();
            var messenger = A.Fake<IMessenger>();
            var customerVM = new CustomerViewModel(customer, messenger);
            customerVM.CustomerId.Should().Be(customer.CustomerId);
            customerVM.CompanyName.Should().Be(customer.CompanyName);
            customerVM.ContactName.Should().Be(customer.ContactName);
            customerVM.ContactTitle.Should().Be(customer.ContactTitle);
            customerVM.Address.Should().Be(customer.Address);
            customerVM.City.Should().Be(customer.City);
            customerVM.Region.Should().Be(customer.Region);
            customerVM.PostalCode.Should().Be(customer.PostalCode);
            customerVM.Country.Should().Be(customer.Country);
            customerVM.Phone.Should().Be(customer.Phone);
            customerVM.Fax.Should().Be(customer.Fax);
        }

        [TestMethod]
        public void DeleteCommand_ShouldSendMessageWithVM()
        {
            var customer = AutoFaker.Generate<Customer>();
            var messenger = WeakReferenceMessenger.Default;
            var customerVM = new CustomerViewModel(customer, messenger);
            object callbackResponse = null;
            var waitEvent = new AutoResetEvent(false);
            WeakReferenceMessenger.Default.Register<ViewModelDeletedMessage>(this, (r, m) =>
            {
                callbackResponse = customerVM;
                waitEvent.Set();
            });
            customerVM.DeleteCommand.Execute(null);
            waitEvent.WaitOne(100);
            callbackResponse.Should().Be(customerVM);
        }

        [TestMethod]
        public void CloseCommand_ShouldSendMessageWithVM()
        {
            var customer = AutoFaker.Generate<Customer>();
            var messenger = WeakReferenceMessenger.Default;
            var customerVM = new CustomerViewModel(customer, messenger);
            object callbackResponse = null;
            var waitEvent = new AutoResetEvent(false);
            WeakReferenceMessenger.Default.Register<WindowClosedMessage>(this, (r, m) =>
            {
                callbackResponse = customerVM;
                waitEvent.Set();
            });
            customerVM.ClosingCommand.Execute(null);
            waitEvent.WaitOne(100);
            callbackResponse.Should().Be(customerVM);
        }
    }
}
