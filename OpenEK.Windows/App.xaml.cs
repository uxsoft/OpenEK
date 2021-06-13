using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using OpenEK.Core;

namespace OpenEK.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            EKManager.connect();
        }
    }
}
