using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpenEK.API
{
    public class Core
    {
        public static Core Instance { get; } = new();

        public bool IsConnected { get; private set; }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
            PInvoke.Release();
            IsConnected = false;
        }

        private int Connect()
        {
            if ((PInvoke.InitEx(5) >= 0 ? 1 : (PInvoke.InitEx(3) >= 0 ? 1 : 0)) == 0)
                return -1;
            IsConnected = true;
            return 0;
        }

        public int Reconnect()
        {
            if (IsConnected)
                Disconnect();
            return Connect();
        }

        public string GetMH()
        {
            if (!IsConnected)
                return "none";
            var bytes = new byte[1024];
            var length = PInvoke.GetMH(ref bytes[0], bytes.Length);
            if (length < 0)
                throw new ApplicationException($"failed to GetMH {length}");
            return Encoding.Default.GetString(bytes, 0, length);
        }

        [HandleProcessCorruptedStateExceptions]
        public FanData GetFan(int num)
        {
            if (!IsConnected)
                return new FanData();
            var numArray = new byte[1024];
            if (PInvoke.GetFan(num, ref numArray[0], numArray.Length) < 0)
                return new FanData();
            return new FanData
            {
                Model = Convert.ToUInt16(numArray[0] << 8 | numArray[1]),
                RatedSpeed = Convert.ToUInt16(numArray[2] << 8 | numArray[3]),
                Speed = Convert.ToUInt16(numArray[4] << 8 | numArray[5]),
                RatedPower = Convert.ToUInt16(numArray[6] << 8 | numArray[7]),
                Power = Convert.ToUInt16(numArray[8] << 8 | numArray[9]),
                Load = Convert.ToUInt16(numArray[10] << 8 | numArray[11]),
                Pwm = Convert.ToUInt16(numArray[12] << 8 | numArray[13])
            };
        }

        public int GetTemperature(int num)
        {
            if (!IsConnected)
                return 0;
            var numArray = new byte[1024];
            var length = PInvoke.GetTemperature(num, ref numArray[0], numArray.Length);
            if (length < 0)
                throw new ApplicationException($"failed to GetTemperature {length}");
            return 0;
        }

        public PumpData GetWaterPump()
        {
            if (!IsConnected)
                return new PumpData();
            var buffer = new byte[1024];
            if (PInvoke.GetWaterPump(ref buffer[0], buffer.Length) < 0)
                return new PumpData();
            return new PumpData
            {
                Model = Convert.ToUInt16(buffer[0] << 8 | buffer[1]),
                RatedSpeed = Convert.ToUInt16(buffer[2] << 8 | buffer[3]),
                Speed = Convert.ToUInt16(buffer[4] << 8 | buffer[5]),
                RatedPower = Convert.ToUInt16(buffer[6] << 8 | buffer[7]),
                Power = Convert.ToUInt16(buffer[8] << 8 | buffer[9]),
                Load = Convert.ToUInt16(buffer[10] << 8 | buffer[11]),
                Pwm = Convert.ToUInt16(buffer[12] << 8 | buffer[13])
            };
        }

        public int GetLevel()
        {
            if (!IsConnected)
                return 0;
            var buffer = new byte[1024];
            var length = PInvoke.GetLevel(ref buffer[0], buffer.Length);
            if (length < 0)
                throw new ApplicationException($"failed to GetLevel {length}");
            return 0;
        }

        public SensorsData GetSensors()
        {
            if (!IsConnected)
                return new SensorsData();

            var buffer = new byte[1024];

            if (PInvoke.GetSensors(ref buffer[0], buffer.Length) < 0)
                return new SensorsData();

            return new SensorsData
            {
                TempModel1 = Convert.ToUInt16(buffer[0] << 8 | buffer[1]),
                Temp1 = Convert.ToUInt16(buffer[2] << 8 | buffer[3]),
                TempModel2 = Convert.ToUInt16(buffer[4] << 8 | buffer[5]),
                Temp2 = Convert.ToUInt16(buffer[6] << 8 | buffer[7]),
                TempModel3 = Convert.ToUInt16(buffer[8] << 8 | buffer[9]),
                Temp3 = Convert.ToUInt16(buffer[10] << 8 | buffer[11]),
                FlowModel = Convert.ToUInt16(buffer[12] << 8 | buffer[13]),
                FlowValue = Convert.ToUInt16(buffer[14] << 8 | buffer[15]),
                LevelModel = Convert.ToUInt16(buffer[16] << 8 | buffer[17]),
                LevelValue = Convert.ToUInt16(buffer[18] << 8 | buffer[19])
            };
        }

        private int SetFanPWM(byte num, ushort pwm)
        {
            if (!IsConnected)
                return 0;
            var length = PInvoke.SetFanPWM(num, pwm);
            if (length < 0)
                throw new ApplicationException($"failed to GetLevel {length}");
            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        public int SetPump(
            ushort model,
            ushort ratedSpeed,
            ushort speed,
            ushort ratedPower,
            ushort power,
            ushort load,
            ushort setting)
        {
            if (!IsConnected)
                return 0;
            try
            {
                return PInvoke.SetPump(model, ratedSpeed, speed, ratedPower, power, load, setting);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public string GetHardwareVersion()
        {
            if (!IsConnected)
                return "";
            var buffer = new byte[1024];
            var length = PInvoke.GetHWVersion(ref buffer[0], buffer.Length);
            if (length < 0)
                throw new ApplicationException($"failed to GetHardwareVersion {length}");
            return Encoding.Default.GetString(buffer, 0, length);
        }

        public string GetSoftwareVersion()
        {
            if (!IsConnected)
                return "sw";
            var buffer = new byte[1024];
            var length = PInvoke.GetSWVersion(ref buffer[0], buffer.Length);
            if (length < 0)
                throw new ApplicationException($"failed to GetSoftwareVersion {length}");
            return Encoding.Default.GetString(buffer, 0, length);
        }

        public int SetLED(
            byte mode,
            byte color,
            byte speed,
            byte brightness,
            byte red,
            byte green,
            byte blue,
            byte rgbReload)
        {
            if (IsConnected)
                return PInvoke.SetSpecialLED(1, mode, color, speed, brightness, red, green, blue, rgbReload, 0, 0, 0);
            else return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        public int SetFanData(
            byte num,
            ushort model,
            ushort ratedSpeed,
            ushort speed,
            ushort ratedPower,
            ushort power,
            ushort load,
            ushort pwm)
        {
            if (!IsConnected && Thread.CurrentThread.IsAlive)
                return 0;

            try
            {
                return PInvoke.SetFanData(num, model, ratedSpeed, speed, ratedPower, power, load, pwm);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public LED_Data GetLed()
        {
            if (!IsConnected)
                return new LED_Data();
            byte[] numArray = new byte[1024];
            var led = PInvoke.GetLED(ref numArray[0], numArray.Length);
            if (led < 0)
                throw new ApplicationException($"failed to GetLED {led}");
            return new LED_Data
            {
                Model = numArray[0],
                Mode = numArray[1],
                Color = numArray[2],
                Speed = numArray[3],
                Brightness = numArray[4],
                Red = numArray[5],
                Green = numArray[6],
                Blue = numArray[7],
                RgbReload = numArray[8],
                LastMode = numArray[9],
                LastColor = numArray[10],
                LastSpd = numArray[11],
                LastBrightness = numArray[12],
                LastRed = numArray[13],
                LastGreen = numArray[14],
                LastBlue = numArray[15],
                Reserve0 = numArray[16]
            };
        }

        public int Send(byte[] buf)
        {
            if (!IsConnected)
                return 0;
            return PInvoke.Send(buf, buf.Length);
        }

        public class FanData
        {
            public ushort Model;
            public ushort RatedSpeed;
            public ushort Speed;
            public ushort RatedPower;
            public ushort Power;
            public ushort Load;
            public ushort Pwm;
        }

        public class PumpData
        {
            public ushort Model;
            public ushort RatedSpeed;
            public ushort Speed;
            public ushort RatedPower;
            public ushort Power;
            public ushort Load;
            public ushort Pwm;
        }

        public class SensorsData
        {
            public ushort TempModel1;
            public ushort Temp1;
            public ushort TempModel2;
            public ushort Temp2;
            public ushort TempModel3;
            public ushort Temp3;
            public ushort FlowModel;
            public ushort FlowValue;
            public ushort LevelModel;
            public ushort LevelValue;
        }

        public class LED_Data
        {
            public byte Model;
            public byte Mode;
            public byte Color;
            public byte Speed;
            public byte Brightness;
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte RgbReload;
            public byte LastMode;
            public byte LastColor;
            public byte LastSpd;
            public byte LastBrightness;
            public byte LastRed;
            public byte LastGreen;
            public byte LastBlue;
            public byte Reserve0;
        }

        private class PInvoke
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
                ref LEDColor buf,
                int count);

            [DllImport("UsbDll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SetSingleLED(int mode, int id, LEDColor color);

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
}