namespace OpenEK.Core.Native

open System
open System.Runtime.ExceptionServices
open System.Text

type EKConnectBus() =

    member val IsConnected = false with get, set

    [<HandleProcessCorruptedStateExceptions>]
    member x.Disconnect() =
        if x.IsConnected then
            PInvoke.Release() |> ignore
            x.IsConnected <- false

    [<HandleProcessCorruptedStateExceptions>]
    member x.Connect() =
        if PInvoke.InitEx(5) >= 0 then
            x.IsConnected <- true
            1
        else if PInvoke.InitEx(3) >= 0 then
            x.IsConnected <- true
            1
        else
            -1

    member x.Reconnect() =
        if x.IsConnected then x.Disconnect()
        x.Connect()

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetHardwareVersion() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetHWVersion(&buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to {nameof x.GetHardwareVersion}, received negative length"

                Encoding.Default.GetString(buffer, 0, length)
            else
                String.Empty
        with e ->
            Console.WriteLine(e)
            String.Empty

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetSoftwareVersion() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetSWVersion(&buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to {nameof x.GetSoftwareVersion}, received negative length"

                Encoding.Default.GetString(buffer, 0, length)
            else
                String.Empty
        with e ->
            Console.WriteLine(e)
            String.Empty

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetMh() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetMH(&buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to {nameof x.GetMh}, received negative length"

                Encoding.Default.GetString(buffer, 0, length)
            else
                String.Empty
        with e ->
            Console.WriteLine(e)
            String.Empty

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetFan(port: byte) =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                if PInvoke.GetFan(int port, &buffer.[0], buffer.Length) < 0 then
                    None
                else
                    Some
                        { Model = Convert.ToUInt16(buffer.[0] <<< 8 ||| buffer.[1])
                          RatedSpeed = Convert.ToUInt16(buffer.[2] <<< 8 ||| buffer.[3])
                          Speed = Convert.ToUInt16(buffer.[4] <<< 8 ||| buffer.[5])
                          RatedPower = Convert.ToUInt16(buffer.[6] <<< 8 ||| buffer.[7])
                          Power = Convert.ToUInt16(buffer.[8] <<< 8 ||| buffer.[9])
                          Load = Convert.ToUInt16(buffer.[10] <<< 8 ||| buffer.[11])
                          Pwm = Convert.ToUInt16(buffer.[12] <<< 8 ||| buffer.[13]) }
            else
                None
        with e ->
            Console.WriteLine(e)
            None

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetFans(includePump: bool) =
        let fanCount = if includePump then 6uy else 5uy

        let fans =
            [ 1uy .. fanCount ]
            |> List.map (fun i -> (i, x.GetFan i))

        fans
        |> List.filter
            (function
            | (_, None) -> false
            | (_, Some data) -> data.Speed > 0us)
        |> List.map (fun (i, data) -> (i, Option.get data))
        |> Map.ofList

    [<HandleProcessCorruptedStateExceptions>]
    member x.SetFan (fan: FanData) (port: byte) (pwm: uint16) =
        try
            if x.IsConnected then // not Thread.CurrentThread.IsAlive
                PInvoke.SetFanData(port, fan.Model, fan.RatedSpeed, fan.Speed, fan.RatedPower, fan.Power, fan.Load, pwm)
            else
                0
        with e ->
            Console.WriteLine(e)
            -1

    /// <summary>
    /// Doesn't work for some reason
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pwm"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    member x.SetFanPwm (index: byte) (pwm: uint16) =
        if x.IsConnected then
            let length = PInvoke.SetFanPWM(index, pwm)

            if length < 0 then
                failwith $"failed to {nameof x.SetFanPwm}, received negative length"
            else
                1
        else
            0

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetPump() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                if PInvoke.GetWaterPump(&buffer.[0], buffer.Length) < 0 then
                    None
                else
                    Some
                        { Model = Convert.ToUInt16(buffer.[0] <<< 8 ||| buffer.[1])
                          RatedSpeed = Convert.ToUInt16(buffer.[2] <<< 8 ||| buffer.[3])
                          Speed = Convert.ToUInt16(buffer.[4] <<< 8 ||| buffer.[5])
                          RatedPower = Convert.ToUInt16(buffer.[6] <<< 8 ||| buffer.[7])
                          Power = Convert.ToUInt16(buffer.[8] <<< 8 ||| buffer.[9])
                          Load = Convert.ToUInt16(buffer.[10] <<< 8 ||| buffer.[11])
                          Pwm = Convert.ToUInt16(buffer.[12] <<< 8 ||| buffer.[13]) }
            else
                None
        with e ->
            Console.WriteLine(e)
            None

    /// <summary>
    /// Used to adjust pump speed (pwm). Pump must be in fan port #6.
    /// </summary>
    /// <param name="pwm">Value between 0 and 100</param>
    /// <returns></returns>
    [<HandleProcessCorruptedStateExceptions>]
    member x.SetPump (pump: FanData) (pwm: uint16) =
        try
            if x.IsConnected then
                PInvoke.SetPump(pump.Model, pump.RatedSpeed, pump.Speed, pump.RatedPower, pump.Power, pump.Load, pwm)
            else
                0
        with e -> -1

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetLed() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetLED(&buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to {nameof x.GetLed}, received negative length"
                else
                    Some
                        { Model = buffer.[0]
                          Mode = LanguagePrimitives.EnumOfValue buffer.[1]
                          Color = buffer.[2]
                          Speed = buffer.[3]
                          Brightness = buffer.[4]
                          Red = buffer.[5]
                          Green = buffer.[6]
                          Blue = buffer.[7]
                          RgbReload = buffer.[8]
                          LastMode = LanguagePrimitives.EnumOfValue buffer.[9]
                          LastColor = buffer.[10]
                          LastSpd = buffer.[11]
                          LastBrightness = buffer.[12]
                          LastRed = buffer.[13]
                          LastGreen = buffer.[14]
                          LastBlue = buffer.[15]
                          Reserve0 = buffer.[16] }
            else
                None
        with e ->
            Console.WriteLine(e)
            None

    [<HandleProcessCorruptedStateExceptions>]
    member x.SetLed (mode: LedMode) (speed: byte) (brightness: byte) (red: byte) (green: byte) (blue: byte) =
        try
            let color =
                match mode with
                | LedMode.Fading -> Byte.MaxValue
                | LedMode.CoveringMarquee -> Byte.MaxValue
                | LedMode.SpectrumWave -> Byte.MaxValue
                | _ -> Byte.MinValue

            if x.IsConnected then
                PInvoke.SetSpecialLED(1uy, (byte) mode, color, speed, brightness, red, green, blue, 0uy, 0uy, 0uy, 0uy)
            else
                0

        with e ->
            Console.WriteLine(e)
            -1

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetTemperature(port: int) =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetTemperature(port, &buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to GetTemperature length"
                else
                    1
            else
                0
        with e ->
            Console.WriteLine(e)
            -1

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetLevel() =
        try
            if x.IsConnected then
                let buffer = Array.create 1024 0uy

                let length =
                    PInvoke.GetLevel(&buffer.[0], buffer.Length)

                if length < 0 then
                    failwith $"failed to {nameof x.GetLevel} length"
                else
                    1
            else
                0
        with e ->
            Console.WriteLine(e)
            -1

    [<HandleProcessCorruptedStateExceptions>]
    member x.GetSensors() =
        try
            if x.IsConnected then
                let buffer = Array.create<byte> 1024 0uy

                if PInvoke.GetSensors(&buffer.[0], buffer.Length) < 0 then
                    None
                else
                    Some
                        { TempModel1 = Convert.ToUInt16(buffer.[0] <<< 8 ||| buffer.[1])
                          Temp1 = Convert.ToUInt16(buffer.[2] <<< 8 ||| buffer.[3])
                          TempModel2 = Convert.ToUInt16(buffer.[4] <<< 8 ||| buffer.[5])
                          Temp2 = Convert.ToUInt16(buffer.[6] <<< 8 ||| buffer.[7])
                          TempModel3 = Convert.ToUInt16(buffer.[8] <<< 8 ||| buffer.[9])
                          Temp3 = Convert.ToUInt16(buffer.[10] <<< 8 ||| buffer.[11])
                          FlowModel = Convert.ToUInt16(buffer.[12] <<< 8 ||| buffer.[13])
                          FlowValue = Convert.ToUInt16(buffer.[14] <<< 8 ||| buffer.[15])
                          LevelModel = Convert.ToUInt16(buffer.[16] <<< 8 ||| buffer.[17])
                          LevelValue = Convert.ToUInt16(buffer.[18] <<< 8 ||| buffer.[19]) }
            else
                None
        with e ->
            Console.WriteLine(e)
            None

    [<HandleProcessCorruptedStateExceptions>]
    member x.Send(buffer: byte array) =
        try
            if x.IsConnected then
                PInvoke.Send(buffer, buffer.Length)
            else
                0
        with e ->
            Console.WriteLine(e)
            -1
