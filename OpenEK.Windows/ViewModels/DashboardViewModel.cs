using System;
using System.Linq;
using Fonderie;
using OpenEK.Core.Native;
using Microsoft.FSharp.Core;
using System.Windows.Documents;
using System.Collections.Generic;

namespace OpenEK.Windows.ViewModels
{
    public partial class DashboardViewModel
    {
        [GeneratedProperty] string _cpu = "";
        [GeneratedProperty] string _gpu = "";
        [GeneratedProperty] string _pump = "";
        [GeneratedProperty] string _fan1Title = "";
        [GeneratedProperty] string _fan1Value = "";
        [GeneratedProperty] string _fan2Title = "";
        [GeneratedProperty] string _fan2Value = "";
        [GeneratedProperty] string _fan3Title = "";
        [GeneratedProperty] string _fan3Value = "";
        [GeneratedProperty] string _fan4Title = "";
        [GeneratedProperty] string _fan4Value = "";

        [GeneratedProperty] ushort _fanPwm;
        [GeneratedProperty] ushort _pumpPwm;
        [GeneratedProperty] bool _autoFanAdjust;
 
        [GeneratedProperty]
        List<ushort> _pwmSteps =
            Enumerable.Range(0, 11).Select(i => (ushort)(i * 10)).ToList();

        public void Start()
        {
            EK.Manager.Start();
            EK.Manager.OnDataUpdated += FanManagerOnDataUpdated;
            PropertyChanged += DashboardViewModel_PropertyChanged;
        }

        private void DashboardViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FanPwm) && 
                EK.Manager.Fans.Count > 0 && 
                FanPwm != EK.Manager.Fans.First().Value.Pwm)
            {
                EK.Manager.Send(EkCommand.NewSetFansPwm(FanPwm));
            }
            if (e.PropertyName == nameof(PumpPwm) &&
                PumpPwm != EK.Manager.Pump.Pwm)
            {
                EK.Manager.Send(EkCommand.NewSetPumpPwm(PumpPwm));
            }
        }

        void FanManagerOnDataUpdated(object? sender, Unit e)
        {
            HardwareMonitor.Update();

            var tCpu = HardwareMonitor.GetCpuTemperature("Core Average");
            var tGpu = HardwareMonitor.GetGpuTemperature("GPU Core");

            Cpu = $"{tCpu:F1} °C";
            Gpu = $"{tGpu:F1} °C";
            Pump = $"{EK.Manager.Pump.Pwm} @ {EK.Manager.Pump.Speed} rpm";

            PumpPwm = EK.Manager.Pump.Pwm;
            if (EK.Manager.Fans.Count > 0)
                FanPwm = EK.Manager.Fans.First().Value.Pwm;

            var fanSetters = new Action<string, string>[] {
                (t, v) => { Fan1Value = v; Fan1Title = t; },
                (t, v) => { Fan2Value = v; Fan2Title = t; },
                (t, v) => { Fan3Value = v; Fan3Title = t; },
                (t, v) => { Fan4Value = v; Fan4Title = t; },
            };

            for (var i = 0; i < Math.Min(EK.Manager.Fans.Count, fanSetters.Length - 1); i++)
            {
                var fan = EK.Manager.Fans.ElementAt(i);
                var setter = fanSetters[i];

                setter($"FAN{fan.Key}", $"{fan.Value.Pwm} @ {fan.Value.Speed} rpm");
            }
        }
    }
}
