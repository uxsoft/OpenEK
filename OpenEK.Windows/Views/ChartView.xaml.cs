using System.Windows.Controls;
using OpenEK.Windows.ViewModels;

namespace OpenEK.Windows.Views
{
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            DataContext = new ChartViewModel();
        }
    }
}
