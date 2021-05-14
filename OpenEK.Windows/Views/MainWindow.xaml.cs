using System;
using System.Windows;
using System.Windows.Controls;
using Windows.UI;
using ModernWpf.Controls;

namespace OpenEK.Windows.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigation.SelectedItem = "Dashboard";
        }

        private void Navigation_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var views = new UserControl[] { DashboardView, LightsView };
            UserControl selectedView = args.InvokedItem switch
            {
                "Dashboard" => DashboardView,
                "Lights" => LightsView,
                _ => DashboardView
            };

            Navigation.Header = args.InvokedItem;
            
            foreach (var view in views) 
                view.Visibility = view != selectedView ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}