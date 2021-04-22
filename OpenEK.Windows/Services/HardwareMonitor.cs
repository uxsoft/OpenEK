using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace OpenEK
{
    public static class HardwareMonitor
    {
        static HardwareMonitor()
        {
            Computer.Open();
            
        }

        public static Computer Computer { get; } = new()
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true
        };

        public static void Update()
        {
            foreach (var hardware in Computer.Hardware)
            {
                hardware.Update();
            }
        }

        public static string GetCpuName()
        {
            var cpu = Computer.Hardware
                .SingleOrDefault(hw => hw.HardwareType is HardwareType.Cpu);
            return cpu?.Name ?? "";
        }

        public static IEnumerable<string> GetCpuTemperatureSensors()
        {
            var cpu = Computer.Hardware
                .SingleOrDefault(hw => hw.HardwareType is HardwareType.Cpu);
            return cpu?.Sensors?.Select(s => s.Name) ?? Array.Empty<string>();
        }
        
        public static double GetCpuTemperature(string sensorName)
        {
            var cpu = Computer.Hardware
                .SingleOrDefault(hw => hw.HardwareType is HardwareType.Cpu);
            var sensor = cpu?.Sensors?.SingleOrDefault(s => 
                s.SensorType == SensorType.Temperature && 
                s.Name == sensorName);
            
            sensor.ValuesTimeWindow = TimeSpan.FromSeconds(1);
            
            return sensor?.Value ?? 0;
        }
        
        public static string GpuName()
        {
            var cpu = Computer.Hardware.SingleOrDefault(hw => 
                hw.HardwareType is HardwareType.GpuAmd or HardwareType.GpuNvidia);
            return cpu?.Name ?? "";
        }

        public static IEnumerable<string> GetGpuTemperatureSensors()
        {
            var cpu = Computer.Hardware
                .SingleOrDefault(hw => hw.HardwareType is HardwareType.GpuAmd or HardwareType.GpuNvidia);
            return cpu?.Sensors?.Select(s => s.Name) ?? Array.Empty<string>();
        }
        
        public static double GetGpuTemperature(string sensorName)
        {
            var cpu = Computer.Hardware
                .SingleOrDefault(hw => hw.HardwareType is HardwareType.GpuAmd or HardwareType.GpuNvidia);
            var sensor = cpu?.Sensors?.SingleOrDefault(s => 
                s.SensorType == SensorType.Temperature && 
                s.Name == sensorName);
            return sensor?.Value ?? 0;
        }
    }
}