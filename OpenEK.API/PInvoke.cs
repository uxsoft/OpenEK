using System.Runtime.InteropServices;

namespace OpenEK.API
{
    internal static class PInvoke
    {
        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init();

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitEx(int num);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Release();

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetHWVersion(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSWVersion(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFanMode(int id);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFanType(int id);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFanSpeed(int id);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFanPower(int id);

        [DllImport("UsbDll.dll", EntryPoint = "SetLED", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSpecialLED(
            byte model,
            byte mode,
            byte color,
            byte speed,
            byte brightness,
            byte red,
            byte green,
            byte blue,
            byte rgbReload,
            byte reserve0,
            byte reserve1,
            byte reserve2);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetLEDALL(
            int mode,
            int speed,
            int brightness,
            ref LedColor buf,
            int count);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSingleLED(int mode, int id, LedColor color);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMH(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFan(int num, ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTemperature(int num, ref byte buf, int len);

        [DllImport("UsbDll.dll", EntryPoint = "GetPump", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetWaterPump(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetLevel(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSensors(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetFanPWM(byte num, ushort pwm);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPump(
            ushort model,
            ushort ratedSpeed,
            ushort speed,
            ushort ratedPower,
            ushort power,
            ushort load,
            ushort setting);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetFanData(
            byte num,
            ushort model,
            ushort ratedSpeed,
            ushort speed,
            ushort ratedPower,
            ushort power,
            ushort load,
            ushort pwm);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetLED(ref byte buf, int len);

        [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Send([MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len);
    }
}