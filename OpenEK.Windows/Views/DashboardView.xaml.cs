using System.Windows;
using System.Windows.Controls;
using OpenEK.Windows.ViewModels;

namespace OpenEK.Windows.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }
    }
}