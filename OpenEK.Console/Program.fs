open System
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
    printfn $"PUMP: {EK.Manager.Pump.Pwm}pwm {EK.Manager.Pump.Speed}rpm"
    for fan in EK.Manager.Fans do
        printfn $"FAN{fan.Key}: {fan.Value.Pwm}pwm {fan.Value.Speed}rpm"

let setPump pwm =
    EK.Manager.Send (SetPumpPwm pwm)
    refresh()

let setFans pwm =
    EK.Manager.Send (SetFansPwm pwm)
    refresh()

[<EntryPoint>]
let main argv =
    EK.Manager.Start false |> ignore
 
    while true do
        match Console.ReadLine() with
        | RegEx "fans (\d+)" [pwm] -> setFans (uint16 pwm)
        | RegEx "pump (\d+)" [pwm] -> setPump (uint16 pwm)
        | _ -> refresh()
        
    0 // return an integer exit code