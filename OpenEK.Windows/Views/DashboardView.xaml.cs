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
    }
}