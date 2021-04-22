using System;

namespace OpenEK
{
    public static class FanCurve
    {
        public static double LinearPwm(double temperature)
        {
            var tMin = 30;
            var tMax = 90;
            var pwm = Math.Max(0, temperature - tMin) / (tMax - tMin);

            return Math.Min(pwm, 99);
        }
    }
}