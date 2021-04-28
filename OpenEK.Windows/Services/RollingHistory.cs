using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenEK
{
    public class RollingHistory
    {
        public int HistoryLength { get; set; } = 10;
        public ObservableCollection<double> Readings { get; } = new();

        public void AddReading(double value)
        {
            Readings.Add(value);
            if (Readings.Count > 1 && Readings.Count > HistoryLength)
                Readings.RemoveAt(0);
        }
    }
}