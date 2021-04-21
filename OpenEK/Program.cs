using System;
using System.Timers;
using OpenEK.API;

namespace OpenEK
{
    public class Program
    {
        public static RollingHistory CpuTemperatureHistory { get; set; } = new();
        public static RollingHistory GpuTemperatureHistory { get; set; } = new();

        public static void Main(string[] args)
        {
            var timer = new Timer(1000)
            {
                AutoReset = true
            };
            timer.Elapsed += TimerOnElapsed;
            timer.Start();

            EkConnect.Instance.Reconnect();

            Console.ReadKey();
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            HardwareMonitor.Update();
            
            var tCpu = HardwareMonitor.GetCpuTemperature("Core Average");
            CpuTemperatureHistory.AddReading(tCpu);
            var tGpu = HardwareMonitor.GetGpuTemperature("GPU Core");
            GpuTemperatureHistory.AddReading(tGpu);
            
            var fans = FanManager.AdjustFans(tCpu);
            
            Console.Clear();
            Console.WriteLine($"CPU: {tCpu:F1}°C\tGPU: {tGpu:F1}°C");

            foreach (var fan in fans)
            {
                Console.Write($"FAN{fan.Key}: {fan.Value.Pwm} => {fan.Value.Speed}rpm\t");
            }
            Console.WriteLine();
        }
    }
}