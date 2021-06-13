open System
open System.Drawing
open System.Text.RegularExpressions
open System.Threading
open OpenEK.Core
open OpenEK.Core.EK
open OpenEK.Core.EK.Commands

let (|RegEx|_|) regex str =
   let m = Regex(regex).Match(str)
   if m.Success
   then Some (List.tail [ for x in m.Groups -> x.Value ])
   else None

let refresh() =
    let cpu = EKService.cpu()
    let gpu = EKService.gpu()
    
    EKService.refreshState()
    
    Thread.Sleep(200)
    Console.Clear()
    printf $"CPU: {cpu:F1}\t GPU: {gpu:F1}\t"
    printfn $"PUMP: {EKService.deviceState.Pump.Pwm}pwm/{EKService.deviceState.Pump.Speed}rpm"
    for fan in EKService.deviceState.Fans do
        printf $"FAN{fan.Key}: {fan.Value.Pwm}pwm/{fan.Value.Speed}rpm\t"
    printfn ""

let setPump pwm =
    EKService.queueCommand (SetPumpPwm pwm)
    refresh()

let setFans pwm =
    EKService.queueCommand (SetFansPwm pwm)
    refresh()
    
let setLightMode modeName =
    let success, mode = Enum.TryParse<LedMode>(modeName)
    EKService.queueCommand (SetLedMode mode)
    refresh()

let setLightColor color =
    EKService.queueCommand (SetLedColor color)
    refresh()

[<EntryPoint>]
let main argv =
    EKService.connect()
 
    refresh()
    
    let mutable userRequestedExit = false
    while not userRequestedExit do
        match Console.ReadLine() with
        | RegEx "fans (\d+)" [pwm] -> setFans (uint16 pwm)
        | RegEx "pump (\d+)" [pwm] -> setPump (uint16 pwm)
        | RegEx "light ([A-z]+)" [mode] -> setLightMode mode
        | RegEx "color (#[a-fA-F0-9]{6,8})" [color] ->
            ColorTranslator.FromHtml(color) |> setLightColor
        | "exit" | "quit" ->
            EKService.disconnect()
            userRequestedExit <- true
        | _ -> refresh()
        
    0 // return an integer exit code