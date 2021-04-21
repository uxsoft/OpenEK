using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;

namespace OpenEK.API
{
    public class EkConnect
    {
        public static EkConnect Instance { get; } = new();

        public bool IsConnected { get; private set; }

        [HandleProcessCorruptedStateExceptions]
        public void Disconnect()
        {
            if (!IsConnected)
                return;
            PInvoke.Release();
            IsConnected = false;
        }

        [HandleProcessCorruptedStateExceptions]
        private int Connect()
        {
            if (PInvoke.InitEx(5) >= 0)
            {
                IsConnected = true;
                return 1;
            }
            else if (PInvoke.InitEx(3) >= 0)
            {
                IsConnected = true;
                return 1;
            }
            else
                return -1;
        }

        public int Reconnect()
        {
            if (IsConnected)
                Disconnect();
            return Connect();
        }

        [HandleProcessCorruptedStateExceptions]
        public string GetHardwareVersion()
        {
            try
            {
                if (!IsConnected)
                    return string.Empty;

                var buffer = new byte[1024];
                var length = PInvoke.GetHWVersion(ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetHardwareVersion {length}");
                return Encoding.Default.GetString(buffer, 0, length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public string GetSoftwareVersion()
        {
            try
            {
                if (!IsConnected)
                    return string.Empty;

                var buffer = new byte[1024];
                var length = PInvoke.GetSWVersion(ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetSoftwareVersion {length}");
                return Encoding.Default.GetString(buffer, 0, length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public string GetMh()
        {
            try
            {
                if (!IsConnected)
                    return string.Empty;

                var buffer = new byte[1024];
                var length = PInvoke.GetMH(ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetMH {length}");
                return Encoding.Default.GetString(buffer, 0, length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public FanData? GetFan(int index)
        {
            try
            {
                if (!IsConnected)
                    return null;

                var buffer = new byte[1024];
                if (PInvoke.GetFan(index, ref buffer[0], buffer.Length) < 0)
                    return null;

                return new FanData
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public Dictionary<int, FanData> GetFans(bool includePump = false)
        {
            return Enumerable
                .Range(1, includePump ? 6 : 5)
                .Select(i => (index: i, data: GetFan(i)))
                .Where(item => item.data != null)
                //.Where(item => item.data.Model == 2)
                .ToDictionary(item => item.index, item => item.data!);
        }

        [HandleProcessCorruptedStateExceptions]
        public int SetFan(byte port, ushort pwm)
        {
            try
            {
                if (!IsConnected && Thread.CurrentThread.IsAlive)
                    return 0;

                var data = GetFan(port);

                if (data == null)
                    return -1;

                return PInvoke.SetFanData(port, data.Model, data.RatedSpeed, data.Speed, data.RatedPower, data.Power,
                    data.Load, pwm);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Doesn't work for some reason
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pwm"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private int SetFanPwm(byte index, ushort pwm)
        {
            if (!IsConnected)
                return 0;
            var length = PInvoke.SetFanPWM(index, pwm);
            if (length < 0)
                throw new ApplicationException($"failed to GetLevel {length}");
            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        public PumpData? GetPump()
        {
            try
            {
                if (!IsConnected)
                    return null;

                var buffer = new byte[1024];
                if (PInvoke.GetWaterPump(ref buffer[0], buffer.Length) < 0)
                    return null;

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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Used to adjust pump speed (pwm). Pump must be in fan port #6.
        /// </summary>
        /// <param name="pwm">Value between 0 and 100</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public int SetPump(ushort pwm)
        {
            if (!IsConnected)
                return 0;
            try
            {
                var data = GetPump();

                if (data == null)
                    return -1;

                return PInvoke.SetPump(data.Model, data.RatedSpeed, data.Speed, data.RatedPower, data.Power, data.Load,
                    pwm);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public LedData? GetLed()
        {
            try
            {
                if (!IsConnected)
                    return null;

                var buffer = new byte[1024];
                var length = PInvoke.GetLED(ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetLED {length}");

                return new LedData
                {
                    Model = buffer[0],
                    Mode = buffer[1],
                    Color = buffer[2],
                    Speed = buffer[3],
                    Brightness = buffer[4],
                    Red = buffer[5],
                    Green = buffer[6],
                    Blue = buffer[7],
                    RgbReload = buffer[8],
                    LastMode = buffer[9],
                    LastColor = buffer[10],
                    LastSpd = buffer[11],
                    LastBrightness = buffer[12],
                    LastRed = buffer[13],
                    LastGreen = buffer[14],
                    LastBlue = buffer[15],
                    Reserve0 = buffer[16]
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public int SetLed(LedMode mode, byte speed, byte brightness, byte red, byte green, byte blue)
        {
            try
            {
                var color = mode switch
                {
                    LedMode.Fading => byte.MaxValue,
                    LedMode.CoveringMarquee => byte.MaxValue,
                    LedMode.SpectrumWave => byte.MaxValue,
                    _ => byte.MinValue
                };

                if (!IsConnected)
                    return 0;

                return PInvoke.SetSpecialLED(1, (byte) mode, color, speed, brightness, red, green, blue, 0, 0, 0, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public int GetTemperature(int num)
        {
            try
            {
                if (!IsConnected)
                    return 0;

                var buffer = new byte[1024];
                var length = PInvoke.GetTemperature(num, ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetTemperature {length}");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public int GetLevel()
        {
            try
            {
                if (!IsConnected)
                    return 0;

                var buffer = new byte[1024];
                var length = PInvoke.GetLevel(ref buffer[0], buffer.Length);
                if (length < 0)
                    throw new ApplicationException($"failed to GetLevel {length}");

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public SensorsData? GetSensors()
        {
            try
            {
                if (!IsConnected)
                    return null;

                var buffer = new byte[1024];

                if (PInvoke.GetSensors(ref buffer[0], buffer.Length) < 0)
                    return null;

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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public int Send(byte[] buffer)
        {
            try
            {
                if (IsConnected)
                    return PInvoke.Send(buffer, buffer.Length);
                else return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}