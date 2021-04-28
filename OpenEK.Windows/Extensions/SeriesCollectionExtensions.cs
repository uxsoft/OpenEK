using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace OpenEK.Windows.Extensions
{
    public static class SeriesCollectionExtensions
    {
        public static void AddAndCut(this ChartValues<ObservableValue> series, double value, int maxValues = 180)
        {
            series.Add(new ObservableValue(value));
            if (series.Count > maxValues)
                series.RemoveAt(0);
        }
    }
}
