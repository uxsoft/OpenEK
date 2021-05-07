using System.Windows.Controls;
using OpenEK.Windows.ViewModels;

namespace OpenEK.Windows.Views
{
    public partial class LightsView : UserControl
    {
        public LightsView()
        {
            InitializeComponent();
            DataContext = new LightsViewModel();
        }
    }
}
