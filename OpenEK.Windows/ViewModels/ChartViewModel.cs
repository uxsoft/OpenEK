using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public partial class ChartViewModel
    {
        public ChartValues<ObservableValue> CpuTemp { get; set; } = new ();
        public ChartValues<ObservableValue> CpuTempNormalised { get; set; } = new();
        public ChartValues<ObservableValue> GpuTemp { get; set; } = new();
        public ChartValues<ObservableValue> GpuTempNormalised { get; set; } = new();
        public ChartValues<ObservableValue> FanSpeed { get; } = new();
        public ChartValues<ObservableValue> PumpSpeed { get; set; } = new();

        public ChartViewModel()
        {
            FanManager.DataUpdated += FanManager_DataUpdated;
        }

        private void FanManager_DataUpdated(object? sender, EventArgs e)
        {
            CpuTemp.AddAndCut(HardwareMonitor.GetCpuTemperature("Core Average"));
            GpuTemp.AddAndCut(HardwareMonitor.GetGpuTemperature("GPU Core"));

            FanSpeed.AddAndCut(FanManager.Fans.Values.Average(f => f.Speed));
            PumpSpeed.AddAndCut(FanManager.Pump.Speed);
        }
    }
}
