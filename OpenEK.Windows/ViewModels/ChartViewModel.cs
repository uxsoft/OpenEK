using System.Linq;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using Microsoft.FSharp.Core;
using OpenEK.Core;
using OpenEK.Windows.Extensions;

namespace OpenEK.Windows.ViewModels
{
    public class ChartViewModel
    {
        GearedValues<ObservableValue> CpuTemp { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        GearedValues<ObservableValue> CpuTempNormalised { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        GearedValues<ObservableValue> GpuTemp { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        GearedValues<ObservableValue> GpuTempNormalised { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        GearedValues<ObservableValue> FanSpeed { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);
        GearedValues<ObservableValue> PumpSpeed { get; } = new GearedValues<ObservableValue>().WithQuality(Quality.Low);

        public ChartViewModel()
        {
            EKManager.OnDataUpdated.AddHandler(OnDataUpdated);
        }

        void OnDataUpdated(object? sender, Unit e)
        {
            CpuTemp.AddAndCut(EKManager.cpu()); //Core Average
            GpuTemp.AddAndCut(EKManager.gpu());

            FanSpeed.AddAndCut(EKManager.deviceState.Fans
                .Select(i => i.Value)
                .Average(f => f.Speed));
            PumpSpeed.AddAndCut(EKManager.deviceState.Pump.Speed);

            CpuTempNormalised.AddAndCut(EKManager.effectiveCpu());
            GpuTempNormalised.AddAndCut(EKManager.effectiveGpu());
        }
    }
}