using System.Linq;
using Accord.Math;
using Accord.Statistics.Models.Regression.Linear;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using Microsoft.FSharp.Core;
using OpenEK.Core.HwInfo;
using OpenEK.Core.Native;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public partial class ChartViewModel
    {
        public GearedValues<ObservableValue> CpuTemp { get; set; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        public GearedValues<ObservableValue> CpuTempNormalised { get; set; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        public GearedValues<ObservableValue> GpuTemp { get; set; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        public GearedValues<ObservableValue> GpuTempNormalised { get; set; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        public GearedValues<ObservableValue> FanSpeed { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        public GearedValues<ObservableValue> PumpSpeed { get; set; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);

        public ChartViewModel()
        {
            EK.Manager.OnDataUpdated += OnDataUpdated;
        }

        private void OnDataUpdated(object? sender, Unit e)
        {
            CpuTemp.AddAndCut(HardwareInfo.GetCpuTemperature("CPU Package").GetValueOrDefault(0)); //Core Average
            GpuTemp.AddAndCut(HardwareInfo.GetGpuTemperature("GPU Core").GetValueOrDefault(0));

            FanSpeed.AddAndCut(EK.Manager.Fans
                .Select(i => i.Value)
                .Average(f => f.Speed));
            PumpSpeed.AddAndCut(EK.Manager.Pump.Speed);

            CpuTempNormalised.AddAndCut(Regress(CpuTemp));
            GpuTempNormalised.AddAndCut(Regress(GpuTemp));
        }
    }
}