using System.Windows;
using System.Windows.Controls;
using OpenEK.Core.Native;
using OpenEK.Windows.ViewModels;

namespace OpenEK.Windows.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            var vm = new DashboardViewModel();
            vm.Start();
            DataContext = vm;
        }

        private void btnSetPumpPwm_Click(object sender, RoutedEventArgs e)
        {
            EK.Manager.Send(EkCommand.NewSetFansPwm((ushort) txtTargetPwm.Value));
        }

        private void btnSetFanPwm_Click(object sender, RoutedEventArgs e)
        {
            EK.Manager.Send(EkCommand.NewSetPumpPwm((ushort) txtTargetPwm.Value));
        }
    }
}