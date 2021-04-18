namespace OpenEK.API
{
    public static class PumpManager
    {
        public static void GetAllRgbDevices()
        {
            EKConnect.Instance.SetPump(0);
            EKConnect.Instance.GetPump()
            var data = EKConnect.Instance.GetFans();

        }

        public static void SetRgb()
        {
            EKConnect.Instance.SetLed(LedMode.Breathing, 50, 10, 255, 0, 255);
        }
    }
}