open System
open System.Drawing
open System.Text.RegularExpressions
open System.Threading
open OpenEK.Core.HwInfo
open OpenEK.Core.Native

let (|RegEx|_|) regex str =
   let m = Regex(regex).Match(str)
   if m.Success
   then Some (List.tail [ for x in m.Groups -> x.Value ])
   else None

let refresh() =
    let cpu = HardwareMonitor.getCpuTemperature "CPU Package"
    let gpu = HardwareMonitor.getGpuTemperature "GPU Core"
    EK.Manager.Update()
    Thread.Sleep(200)
    Console.Clear()
    printf $"CPU: {cpu:F1}\t GPU: {gpu:F1}\t"
    printfn $"PUMP: {EK.Manager.Pump.Pwm}pwm/{EK.Manager.Pump.Speed}rpm"
    for fan in EK.Manager.Fans do
        printf $"FAN{fan.Key}: {fan.Value.Pwm}pwm/{fan.Value.Speed}rpm\t"
    printfn ""

let setPump pwm =
    EK.Manager.Send (SetPumpPwm pwm)
    refresh()

let setFans pwm =
    EK.Manager.Send (SetFansPwm pwm)
    refresh()
    
let setLightMode modeName =
    let success, mode = Enum.TryParse<LedMode>(modeName)
    EK.Manager.Send (SetLedMode mode)
    refresh()

let setLightColor color =
    EK.Manager.Send (SetLedColor color)
    refresh()

[<EntryPoint>]
let main argv =
    EK.Manager.ConnectToEkConnect()
 
    while true do
        match Console.ReadLine() with
        | RegEx "fans (\d+)" [pwm] -> setFans (uint16 pwm)
        | RegEx "pump (\d+)" [pwm] -> setPump (uint16 pwm)
        | RegEx "light ([A-z]+)" [mode] -> setLightMode mode
        | RegEx "color (#[a-fA-F0-9]{6})" [color] -> ColorTranslator.FromHtml(color) |> setLightColor
        | _ -> refresh()
        
    0 // return an integer exit code