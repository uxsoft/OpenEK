using System.Windows;
using System.Windows.Controls;
using Windows.UI;

namespace OpenEK.Windows.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void navigation_ItemInvoked(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var pageType = args.InvokedItem switch
            {
                "Dashboard" => typeof(DashboardView),
                "Lights" => typeof(LightsView),
                _ => typeof(UserControl)
            };

            Navigation.Header = args.InvokedItem;
            ContentFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
