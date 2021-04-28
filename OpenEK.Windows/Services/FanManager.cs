using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using OpenEK.API;
using System;

namespace OpenEK
{
    public static class FanManager
    {
        public static PumpData Pump { get; set; } = new PumpData();
        public static Dictionary<byte, FanData> Fans { get; set; } = new ();
        public static Timer Timer { get; private set; } = new(1000) { AutoReset = true };
        public static event EventHandler? DataUpdated;
 
        static FanManager()
        {
            Timer.Elapsed += Timer_Elapsed;
        }

        public static void Start()
        {
            Timer.Start();
            _ = EkConnect.Instance.Reconnect();
        }

        public static void Stop()
        {
            Timer.Stop();
            EkConnect.Instance.Disconnect();
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Fans = EkConnect.Instance.GetFans();
            Pump = EkConnect.Instance.GetPump() ?? new PumpData();
            DataUpdated?.Invoke(sender, e);
        }

        public static async Task SetFans(ushort pwm)
        {
            foreach (var fan in Fans)
            {
                EkConnect.Instance.SetFan(fan.Value, fan.Key, pwm);
                await Task.Delay(200);
            }
        }

        public static async Task SetPump(ushort pwm)
        {
            EkConnect.Instance.SetPump(Pump, pwm);
        }

        public static async Task AdjustFans(double temperature)
        {
            var targetPwm = FanCurve.LinearPwm(temperature);

            //foreach (var fanNumber in Enumerable.Range(1, 5))
            //{
            //    EkConnect.Instance.SetFan((byte)fanNumber, (ushort)targetPwm);
            //    await Task.Delay(200);
            //}
        }

        public static async Task AdjustPump(double temperature)
        {
            var targetPwm = FanCurve.LinearPwm(temperature);

        }

        public static void SetRgb()
        {
            EkConnect.Instance.SetLed(LedMode.Breathing, 50, 10, 255, 0, 255);
        }
    }
}