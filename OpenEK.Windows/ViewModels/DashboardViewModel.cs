using System;
using System.Linq;
using OpenEK.Core;
using Microsoft.FSharp.Core;
using System.Collections.Generic;
using System.ComponentModel;
using OpenEK.Core.EK;
using OpenEK.Core.System;

namespace OpenEK.Windows.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            EKManager.OnDataUpdated.AddHandler(FanManagerOnDataUpdated);

            (FanPwm as INotifyPropertyChanged).PropertyChanged += FanPwm_PropertyChanged;
            (PumpPwm as INotifyPropertyChanged).PropertyChanged += PumpPwm_PropertyChanged;
        }

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

        void PumpPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (PumpPwm.Value != EKManager.deviceState.Pump.Pwm)
            {
                EKManager.queueCommand(Commands.EkCommand.NewSetPumpPwm(PumpPwm.Value));
            }
        }

        void FanPwm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (EKManager.deviceState.Fans.Count > 0 &&
                FanPwm.Value != EKManager.deviceState.Fans.First().Value.Pwm)
            {
                EKManager.queueCommand(Commands.EkCommand.NewSetFansPwm(FanPwm.Value));
            }
        }

        void FanManagerOnDataUpdated(object? sender, Unit e)
        {
            var tCpu = EKManager.cpu();
            var tGpu = EKManager.gpu();

            Console.WriteLine($"onDataUpdated {tCpu}");

            CpuStatus.Value = $"{tCpu:F1} °C";
            GpuStatus.Value = $"{tGpu:F1} °C";
            PumpStatus.Value = $"{EKManager.deviceState.Pump.Pwm} @ {EKManager.deviceState.Pump.Speed} rpm";
            PumpPwm.Value = EKManager.deviceState.Pump.Pwm;

            if (EKManager.deviceState.Fans.Count > 0)
                FanPwm.Value = EKManager.deviceState.Fans.First().Value.Pwm;

            var fanSetters = new Action<string, string>[]
            {
                (t, v) =>
                {
                    Fan1Status.Value = v;
                    Fan1Label.Value = t;
                },
                (t, v) =>
                {
                    Fan2Status.Value = v;
                    Fan2Label.Value = t;
                },
                (t, v) =>
                {
                    Fan3Status.Value = v;
                    Fan3Label.Value = t;
                },
                (t, v) =>
                {
                    Fan4Status.Value = v;
                    Fan4Label.Value = t;
                },
            };

            for (var i = 0; i < Math.Min(EKManager.deviceState.Fans.Count, fanSetters.Length - 1); i++)
            {
                var (key, value) = EKManager.deviceState.Fans.ElementAt(i);
                var setter = fanSetters[i];

                setter($"FAN{key}", $"{value.Pwm} @ {value.Speed} rpm");
            }

            if (AutoFanAdjust.Value)
                foreach (var command in
                    Cooling.recommendAdjustments(tCpu, FanPwm.Value, PumpPwm.Value))
                    EKManager.queueCommand(command);
        }
    }
}