using System;
using System.Linq;
using OpenEK.Core;
using OpenEK.Core.Native;
using Microsoft.FSharp.Core;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenEK.Windows.ViewModels
{
    public partial class DashboardViewModel
    {
        public State<string> CpuStatus { get; set; } = new("");
        public State<string> GpuStatus { get; set; } = new("");
        public State<string> PumpStatus { get; set; } = new("");
        public State<string> Fan1Status { get; set; } = new("");
        public State<string> Fan2Status { get; set; } = new("");
        public State<string> Fan3Status { get; set; } = new("");
        public State<string> Fan4Status { get; set; } = new("");
        public State<string> Fan1Label { get; set; } = new("");
        public State<string> Fan2Label { get; set; } = new("");
        public State<string> Fan3Label { get; set; } = new("");
        public State<string> Fan4Label { get; set; } = new("");
        public State<ushort> FanPwm { get; set; } = new(0);
        public State<ushort> PumpPwm { get; set; } = new(0);
        public State<bool> AutoFanAdjust { get; set; } = new(true);

        public List<ushort> PwmSteps { get; set; } =
            Enumerable.Range(0, 11).Select(i => (ushort)(i * 10)).ToList();

        public void Start()
        {
            EK.Manager.Start();
            EK.Manager.OnDataUpdated += FanManagerOnDataUpdated;

            (FanPwm as INotifyPropertyChanged).PropertyChanged += FanPwm_PropertyChanged;
            (PumpPwm as INotifyPropertyChanged).PropertyChanged += PumpPwm_PropertyChanged; 
        }

        private void PumpPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (PumpPwm.Value != EK.Manager.Pump.Pwm)
            {
                EK.Manager.Send(EkCommand.NewSetPumpPwm(PumpPwm.Value));
            }
        }

        private void FanPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (EK.Manager.Fans.Count > 0 &&
                FanPwm.Value != EK.Manager.Fans.First().Value.Pwm)
            {
                EK.Manager.Send(EkCommand.NewSetFansPwm(FanPwm.Value));
            }
        }

        void FanManagerOnDataUpdated(object? sender, Unit e)
        {
            HardwareMonitor.Update();

            var tCpu = HardwareMonitor.GetCpuTemperature("Core Average");
            var tGpu = HardwareMonitor.GetGpuTemperature("GPU Core");

            CpuStatus.Value = $"{tCpu:F1} °C";
            GpuStatus.Value = $"{tGpu:F1} °C";
            PumpStatus.Value = $"{EK.Manager.Pump.Pwm} @ {EK.Manager.Pump.Speed} rpm";
            PumpPwm.Value = EK.Manager.Pump.Pwm;

            if (EK.Manager.Fans.Count > 0)
                FanPwm.Value = EK.Manager.Fans.First().Value.Pwm;

            var fanSetters = new Action<string, string>[] {
                (t, v) => { Fan1Status.Value = v; Fan1Label.Value = t; },
                (t, v) => { Fan2Status.Value = v; Fan2Label.Value = t; },
                (t, v) => { Fan3Status.Value = v; Fan3Label.Value = t; },
                (t, v) => { Fan4Status.Value = v; Fan4Label.Value = t; },
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
