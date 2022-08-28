using CustomerApp.ViewModel;
using System;
using System.Collections.Generic;

namespace CustomerApp.Services
{
    public interface INavigationService
    {
        void Navigate(object arg);
        void Close(object arg);
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> viewMapping = new()
        {
            [typeof(MainViewModel)] = typeof(MainWindow),
            [typeof(CustomerViewModel)] = typeof(Detail),
        };

        private readonly Dictionary<object, List<System.Windows.Window>> _openWindows = new();

        public void Navigate(object arg)
        {
            Type vmType = arg.GetType();
            if (viewMapping.ContainsKey(vmType))
            {
                var windowType = viewMapping[vmType];
                var window = (System.Windows.Window)Activator.CreateInstance(windowType);
                window.DataContext = arg;
                if (!_openWindows.ContainsKey(arg))
                {
                    _openWindows.Add(arg, new List<System.Windows.Window>());
                }
                _openWindows[arg].Add(window);
                window.Show();
            }
        }

        public void Close(object arg)
        {
            if (_openWindows.ContainsKey(arg))
            {
                foreach(var window in _openWindows[arg])
                {
                    window.Close();
                }
            }
        }
    }
}
