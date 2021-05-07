using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Fonderie;
using OpenEK.Core.Native;
using Windows.Security.Cryptography.Core;
using Microsoft.FSharp.Core;

namespace OpenEK.Windows.ViewModels
{
    public partial class DashboardViewModel
    {
        [GeneratedProperty] private string _cpu = "";
        [GeneratedProperty] private string _gpu = "";
        [GeneratedProperty] private string _pump = "";
        [GeneratedProperty] private string _fan1Title = "";
        [GeneratedProperty] private string _fan1Value = "";
        [GeneratedProperty] private string _fan2Title = "";
        [GeneratedProperty] private string _fan2Value = "";
        [GeneratedProperty] private string _fan3Title = "";
        [GeneratedProperty] private string _fan3Value = "";
        [GeneratedProperty] private string _fan4Title = "";
        [GeneratedProperty] private string _fan4Value = "";
                   
        public RollingHistory CpuTemperatureHistory { get; set; } = new();
        public RollingHistory GpuTemperatureHistory { get; set; } = new();

        public void Start()
        {
            EK.Manager.Start();
            EK.Manager.DataUpdated += FanManager_DataUpdated;
        }

        void FanManager_DataUpdated(object? sender, Unit e)
        {
            HardwareMonitor.Update();

            var tCpu = HardwareMonitor.GetCpuTemperature("Core Average");
            CpuTemperatureHistory.AddReading(tCpu);
            var tGpu = HardwareMonitor.GetGpuTemperature("GPU Core");
            GpuTemperatureHistory.AddReading(tGpu);

            Cpu = $"{tCpu:F1}°C";
            Gpu = $"{tGpu:F1}°C";
            Pump = $"{EK.Manager.Pump.Pwm} => {EK.Manager.Pump.Speed}rpm";

            var fanSetters = new Action<string, string>[] {
                (t, v) => { Fan1Value = v; Fan1Title = t; },
                (t, v) => { Fan2Value = v; Fan2Title = t; },
                (t, v) => { Fan3Value = v; Fan3Title = t; },
                (t, v) => { Fan4Value = v; Fan4Title = t; },
            };

            for (int i = 0; i < Math.Min(EK.Manager.Fans.Count, fanSetters.Length - 1); i++)
            {
                var fan = EK.Manager.Fans.ElementAt(i);
                var setter = fanSetters[i];

                setter($"FAN{fan.Key}", $"{fan.Value.Pwm} => {fan.Value.Speed}rpm");
            }
        }
    }
}
