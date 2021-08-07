module OpenEK.Core.EK.Device

open System
open System.Diagnostics
open System.Runtime.ExceptionServices
open System.Text

let mutable isConnected = false 

[<HandleProcessCorruptedStateExceptions>]
let connect () =
    isConnected <-
        if PInvoke.InitEx(5) >= 0 then true
        else if PInvoke.InitEx(3) >= 0 then true
        else false
    isConnected

[<HandleProcessCorruptedStateExceptions>]
let disconnect () =
    if isConnected then
        PInvoke.Release() |> ignore
        isConnected <- false

let reconnect () =
    if isConnected then disconnect()
    connect()

[<HandleProcessCorruptedStateExceptions>]
let getHardwareVersion() =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetHWVersion(&buffer.[0], buffer.Length)

            if length < 0 then
                Trace.WriteLine $"failed to getHardwareVersion, received negative length"
                String.Empty
            else Encoding.Default.GetString(buffer, 0, length)
        else
            String.Empty
    with e ->
        Trace.WriteLine e
        String.Empty

[<HandleProcessCorruptedStateExceptions>]
let getSoftwareVersion () =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetSWVersion(&buffer.[0], buffer.Length)

            if length < 0 then
                Trace.WriteLine $"failed to getSoftwareVersion, received negative length"
                String.Empty
            else Encoding.Default.GetString(buffer, 0, length)
        else
            String.Empty
    with e ->
        Trace.WriteLine e
        String.Empty

[<HandleProcessCorruptedStateExceptions>]
let getMh () =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetMH(&buffer.[0], buffer.Length)

            if length < 0 then
                Trace.WriteLine $"failed to getMh, received negative length"
                String.Empty
            else Encoding.Default.GetString(buffer, 0, length)
        else
            String.Empty
    with e ->
        Trace.WriteLine e
        String.Empty

[<HandleProcessCorruptedStateExceptions>]
let getFan port =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            if PInvoke.GetFan(port, &buffer.[0], buffer.Length) < 0 then
                None
            else Some
                    { Model = Convert.ToUInt16(buffer.[0] <<< 8 ||| buffer.[1])
                      RatedSpeed = Convert.ToUInt16(buffer.[2] <<< 8 ||| buffer.[3])
                      Speed = Convert.ToUInt16(buffer.[4] <<< 8 ||| buffer.[5])
                      RatedPower = Convert.ToUInt16(buffer.[6] <<< 8 ||| buffer.[7])
                      Power = Convert.ToUInt16(buffer.[8] <<< 8 ||| buffer.[9])
                      Load = Convert.ToUInt16(buffer.[10] <<< 8 ||| buffer.[11])
                      Pwm = Convert.ToUInt16(buffer.[12] <<< 8 ||| buffer.[13]) }
        else None
    with e ->
        Trace.WriteLine(e)
        None

[<HandleProcessCorruptedStateExceptions>]
let getFans (includePump: bool) =
    let fanCount = if includePump then 6 else 5

    let fans =
        [ 1 .. fanCount ]
        |> List.map (fun i -> (i, getFan i))

    fans
    |> List.filter (function
        | (_, None) -> false
        | (_, Some data) -> data.Speed > 0us)
    |> List.map (fun (i, data) -> (i, Option.get data))
    |> Map.ofList

[<HandleProcessCorruptedStateExceptions>]
let setFan (fan: FanData) port pwm =
    try
        if isConnected then
            PInvoke.SetFanData(port, fan.Model, fan.RatedSpeed, fan.Speed, fan.RatedPower, fan.Power, fan.Load, pwm)
        else 0
    with e ->
        Trace.WriteLine(e)
        -1

[<HandleProcessCorruptedStateExceptions>]
let getPump () =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            if PInvoke.GetWaterPump(&buffer.[0], buffer.Length) < 0 then
                None
            else Some
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
        Trace.WriteLine e
        None

/// <summary>
/// Used to adjust pump speed (pwm). Pump must be in fan port #6.
/// </summary>
/// <param name="pump">Previously retrieved pump data using getPump. All of this data will be written to the device with the new pwm. </param>
/// <param name="pwm">Value between 0 and 100</param>
/// <returns></returns>
[<HandleProcessCorruptedStateExceptions>]
let setPump (pump: FanData) pwm =
    try
        if isConnected then
            PInvoke.SetPump(pump.Model, pump.RatedSpeed, pump.Speed, pump.RatedPower, pump.Power, pump.Load, pwm)
        else 0
    with e -> -1

[<HandleProcessCorruptedStateExceptions>]
let getLed() =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetLED(&buffer.[0], buffer.Length)

            if length < 0 then
                failwith $"failed to getLed, received negative length"
            else Some
                    { Model = buffer.[0]
                      Mode = LanguagePrimitives.EnumOfValue (int buffer.[1])
                      Color = buffer.[2]
                      Speed = buffer.[3]
                      Brightness = buffer.[4]
                      Red = buffer.[5]
                      Green = buffer.[6]
                      Blue = buffer.[7]
                      RgbReload = buffer.[8]
                      LastMode = LanguagePrimitives.EnumOfValue (int buffer.[9])
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
        Trace.WriteLine(e)
        None

[<HandleProcessCorruptedStateExceptions>]
let setLed (mode: LedMode) speed brightness red green blue =
    try
        let color =
            match mode with
            | LedMode.Fading -> Byte.MaxValue
            | LedMode.CoveringMarquee -> Byte.MaxValue
            | LedMode.SpectrumWave -> Byte.MaxValue
            | _ -> Byte.MinValue

        if isConnected then
            PInvoke.SetSpecialLED(1uy, (byte) mode, color, speed, brightness, red, green, blue, 0uy, 0uy, 0uy, 0uy)
        else 0

    with e ->
        Trace.WriteLine(e)
        -1

[<HandleProcessCorruptedStateExceptions>]
let getTemperature port =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetTemperature(port, &buffer.[0], buffer.Length)

            if length < 0 then
                failwith $"failed to GetTemperature, received negative length"
            else 1
        else 0
    with e ->
        Trace.WriteLine e
        -1

[<HandleProcessCorruptedStateExceptions>]
let getLevel () =
    try
        if isConnected then
            let buffer = Array.create 1024 0uy

            let length =
                PInvoke.GetLevel(&buffer.[0], buffer.Length)

            if length < 0 then
                failwith $"failed to getLevel, received negative length"
            else 1
        else 0
    with e ->
        Trace.WriteLine e
        -1

[<HandleProcessCorruptedStateExceptions>]
let getSensors () =
    try
        if isConnected then
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
        Trace.WriteLine e
        None

[<HandleProcessCorruptedStateExceptions>]
let send (buffer: byte array) =
    try
        if isConnected then
            PInvoke.Send(buffer, buffer.Length)
        else 0
    with e ->
        Trace.WriteLine e
        -1