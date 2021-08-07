module OpenEK.Core.EK.PInvoke

open System.Runtime.InteropServices

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int Init()

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int InitEx(int num)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int Release()

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetHWVersion(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetSWVersion(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetFanMode(int id)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetFanType(int id)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetFanSpeed(int id)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetFanPower(int id)

[<DllImport("UsbDll.dll", EntryPoint = "SetLED", CallingConvention = CallingConvention.Cdecl)>]
extern int SetSpecialLED(
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
    byte reserve2)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int SetLEDALL(
    int mode,
    int speed,
    int brightness,
    LedColor& buf,
    int count)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int SetSingleLED(int mode, int id, LedColor color)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetMH(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetFan(int port, byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetTemperature(int port, byte& buf, int len)

[<DllImport("UsbDll.dll", EntryPoint = "GetPump", CallingConvention = CallingConvention.Cdecl)>]
extern int GetWaterPump(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetLevel(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetSensors(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int SetFanPWM(byte port, uint16 pwm)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int SetPump(
    uint16 model,
    uint16 ratedSpeed,
    uint16 speed,
    uint16 ratedPower,
    uint16 power,
    uint16 load,
    uint16 setting)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int SetFanData(
    byte port,
    uint16 model,
    uint16 ratedSpeed,
    uint16 speed,
    uint16 ratedPower,
    uint16 power,
    uint16 load,
    uint16 pwm)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int GetLED(byte& buf, int len)

[<DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)>]
extern int Send([<MarshalAs(UnmanagedType.LPArray)>] byte[] buf, int len)
