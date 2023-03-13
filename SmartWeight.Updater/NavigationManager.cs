using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartWeight.Updater
{
    internal class NavigationManager
    {
        private static Lazy<NavigationManager> _instance
            = new Lazy<NavigationManager>(new NavigationManager());
        public static NavigationManager Instance => _instance.Value;
        public NavigationManager() 
        {
        }
    }
}
