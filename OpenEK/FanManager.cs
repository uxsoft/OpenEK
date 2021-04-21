using System.Collections.Generic;
using System.Linq;
using OpenEK.API;

namespace OpenEK
{
    public static class FanManager
    {
        public static Dictionary<int, FanData> AdjustFans(double temperature)
        {
            var targetPwm = FanCurve.LinearPwm(temperature);
            
            //foreach (var fanNumber in Enumerable.Range(1, 5))
            //     EkConnect.Instance.SetFan((byte) fanNumber, (ushort)targetPwm);

            var data = EkConnect.Instance.GetFans();
            return data;
        }

        public static void SetRgb()
        {
            EkConnect.Instance.SetLed(LedMode.Breathing, 50, 10, 255, 0, 255);
        }
    }
}