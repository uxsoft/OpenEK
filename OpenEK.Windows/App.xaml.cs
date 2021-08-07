using System.Windows;
using OpenEK.Core;

namespace OpenEK.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_OnStartup(object sender, StartupEventArgs e)
        {
            EKManager.connect();
        }
    }
}
