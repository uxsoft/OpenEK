using System;
using System.Linq;
using OpenEK.Core;
using OpenEK.Core.Native;
using Microsoft.FSharp.Core;
using System.Collections.Generic;
using System.ComponentModel;
using OpenEK.Core.HwInfo;

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
            Core.App.Device.Start();
            Core.App.Device.OnDataUpdated += FanManagerOnDataUpdated;

            (FanPwm as INotifyPropertyChanged).PropertyChanged += FanPwm_PropertyChanged;
            (PumpPwm as INotifyPropertyChanged).PropertyChanged += PumpPwm_PropertyChanged;
        }

        private void PumpPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (PumpPwm.Value != Core.App.Device.Pump.Pwm)
            {
                Core.App.Device.Send(EkCommand.NewSetPumpPwm(PumpPwm.Value));
            }
        }

        private void FanPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Core.App.Device.Fans.Count > 0 &&
                FanPwm.Value != Core.App.Device.Fans.First().Value.Pwm)
            {
                Core.App.Device.Send(EkCommand.NewSetFansPwm(FanPwm.Value));
            }
        }

        void FanManagerOnDataUpdated(object? sender, Unit e)
        {
            HardwareInfo.Update();

            var tCpu = HardwareInfo.GetCpuTemperature("Core Average");
            var tGpu = HardwareInfo.GetGpuTemperature("GPU Core");

            CpuStatus.Value = $"{tCpu:F1} °C";
            GpuStatus.Value = $"{tGpu:F1} °C";
            PumpStatus.Value = $"{Core.App.Device.Pump.Pwm} @ {Core.App.Device.Pump.Speed} rpm";
            PumpPwm.Value = Core.App.Device.Pump.Pwm;

            if (Core.App.Device.Fans.Count > 0)
                FanPwm.Value = Core.App.Device.Fans.First().Value.Pwm;

            var fanSetters = new Action<string, string>[] {
                (t, v) => { Fan1Status.Value = v; Fan1Label.Value = t; },
                (t, v) => { Fan2Status.Value = v; Fan2Label.Value = t; },
                (t, v) => { Fan3Status.Value = v; Fan3Label.Value = t; },
                (t, v) => { Fan4Status.Value = v; Fan4Label.Value = t; },
            };

            for (var i = 0; i < Math.Min(Core.App.Device.Fans.Count, fanSetters.Length - 1); i++)
            {
                var fan = Core.App.Device.Fans.ElementAt(i);
                var setter = fanSetters[i];

                setter($"FAN{fan.Key}", $"{fan.Value.Pwm} @ {fan.Value.Speed} rpm");
            }

            if (AutoFanAdjust.Value)
                foreach (var command in Fans.RecommendAdjustments(tCpu.Value, FanPwm.Value, PumpPwm.Value))
                    Core.App.Device.Send(command);
        }
    }
}
