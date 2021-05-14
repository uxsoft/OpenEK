using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics.Models.Regression.Linear;
using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.FSharp.Core;
using OpenEK.Core.Native;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public partial class ChartViewModel
    {
        public ChartValues<ObservableValue> CpuTemp { get; set; } = new();
        public ChartValues<ObservableValue> CpuTempNormalised { get; set; } = new();
        public ChartValues<ObservableValue> GpuTemp { get; set; } = new();
        public ChartValues<ObservableValue> GpuTempNormalised { get; set; } = new();
        public ChartValues<ObservableValue> FanSpeed { get; } = new();
        public ChartValues<ObservableValue> PumpSpeed { get; set; } = new();

        public ChartViewModel()
        {
            EK.Manager.OnDataUpdated += FanManagerOnDataUpdated;
        }

        private void FanManagerOnDataUpdated(object? sender, Unit e)
        {
            CpuTemp.AddAndCut(HardwareMonitor.GetCpuTemperature("CPU Package")); //Core Average
            GpuTemp.AddAndCut(HardwareMonitor.GetGpuTemperature("GPU Core"));

            FanSpeed.AddAndCut(EK.Manager.Fans
                .Select(i => i.Value)
                .Average(f => f.Speed));
            PumpSpeed.AddAndCut(EK.Manager.Pump.Speed);

            CpuTempNormalised.AddAndCut(Regress(CpuTemp));
            GpuTempNormalised.AddAndCut(Regress(GpuTemp));
        }

        public double Regress(ChartValues<ObservableValue> series, int bandwith = 20, int measurementOffset = 10)
        {
            var yValues = series.TakeLast(20).Select(v => v.Value).ToArray();
            var xValues = Enumerable.Range(1, yValues.Length).ToArray();
            var xLog = Elementwise.Log(xValues);

            var ols = new OrdinaryLeastSquares();
            var lr = ols.Learn(xLog, yValues);

            var predicted = lr.Transform(xLog.TakeLast(10).First());
            return predicted;
        }
    }
}