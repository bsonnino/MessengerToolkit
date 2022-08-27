using CustomerApp.ViewModel;
using System;
using System.Collections.Generic;

namespace CustomerApp.Services
{
    public interface INavigationService
    {
        void Navigate(object arg);
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> viewMapping = new()
        {
            [typeof(MainViewModel)] = typeof(MainWindow),
            [typeof(CustomerViewModel)] = typeof(Detail),
        };

        public void Navigate(object arg)
        {
            Type vmType = arg.GetType();
            if (viewMapping.ContainsKey(vmType))
            {
                var windowType = viewMapping[vmType];
                var window = (System.Windows.Window)Activator.CreateInstance(windowType);
                window.DataContext = arg;
                window.Show();
            }
        }

        public void Show(string id)
        {
            throw new NotImplementedException();
        }
    }
}
