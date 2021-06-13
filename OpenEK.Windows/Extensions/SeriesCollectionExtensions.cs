using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Wpf;

namespace OpenEK.Windows.Extensions
{
    public static class SeriesCollectionExtensions
    {
        public static void AddAndCut(this ChartValues<ObservableValue> series, double value, int maxValues = 120)
        {
            series.Add(new ObservableValue(value));
            if (series.Count > maxValues)
                series.RemoveAt(0);
        }

        public static void AddAndCut(this GearedValues<ObservableValue> series, double value, int maxValues = 120)
        {
            series.Add(new ObservableValue(value));
            if (series.Count > maxValues)
                series.RemoveAt(0);
        }
    }
}
