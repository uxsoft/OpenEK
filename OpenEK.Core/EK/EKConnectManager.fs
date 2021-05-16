namespace OpenEK.Core.Native

open System.Drawing
open System.Collections.Generic
open System.Linq
open System.Text
open System.Threading.Tasks
open System.Timers

type EKConnectManager() as x =
    let onDataUpdatedEvent = Event<unit>()
    let onConnectedEvent = Event<unit>()
    let timerElapsedHandler = new ElapsedEventHandler(x.Timer_Elapsed)
    let mutable shouldUpdate = true

    member val Bus = EKConnectBus() with get, set
    member val CommandQueue = Queue<EkCommand>() with get, set
    member val Timer = new Timer(1000., AutoReset = true) with get, set

    [<CLIEvent>]
    member x.OnDataUpdated = onDataUpdatedEvent.Publish

    [<CLIEvent>]
    member x.OnConnected = onConnectedEvent.Publish
    
    member val Pump =
        { Model = 0us
          RatedSpeed = 0us
          Speed = 0us
          RatedPower = 0us
          Power = 0us
          Load = 0us
          Pwm = 0us } with get, set

    member val Fans = Map.empty<byte, FanData> with get, set
    member val LedMode = LedMode.Off with get, set
    member val LedColor = Color.White with get, set
    member val LedSpeed = 0uy with get, set
    member val LedBrightness = 99uy with get, set

    member x.ConnectToEkConnect() =
        x.Bus.Reconnect() |> ignore
        onConnectedEvent.Trigger()
    
    member x.Start() =
        x.ConnectToEkConnect()
        x.Timer.Elapsed.AddHandler(timerElapsedHandler)
        x.Timer.Start()        

    member x.Stop() =
        x.Timer.Stop()
        x.Timer.Elapsed.RemoveHandler(timerElapsedHandler)
        x.Bus.Disconnect()

    member x.Update() =
        x.Fans <- x.Bus.GetFans(false)
        x.Pump <- Option.defaultValue x.Pump (x.Bus.GetPump())

        match x.Bus.GetLed() with
        | None -> ()
        | Some ledData ->
            x.LedMode <- ledData.Mode
            x.LedColor <- Color.FromArgb(255, int ledData.Red, int ledData.Green, int ledData.Blue)
            x.LedSpeed <- ledData.Speed
            x.LedBrightness <- ledData.Brightness
            
        onDataUpdatedEvent.Trigger()
        
    member private x.Timer_Elapsed (sender: obj) (e: ElapsedEventArgs) =
        if shouldUpdate then
            shouldUpdate <- false
            x.Update()
        else
            onDataUpdatedEvent.Trigger()

    member private x.ProcessCommand(command: EkCommand) =
        match command with
        | SetFansPwm pwm ->
            for fan in x.Fans do
                x.Bus.SetFan fan.Value (byte fan.Key) pwm
                |> ignore
                
        | SetPumpPwm pwm ->
            x.Bus.SetPump x.Pump pwm |> ignore
            
        | SetLedColor color ->
            x.LedColor <- color
            x.Bus.SetLed x.LedMode x.LedSpeed x.LedBrightness x.LedColor.R x.LedColor.G x.LedColor.B
            |> ignore
            
        | SetLedMode mode ->
            x.LedMode <- mode
            x.Bus.SetLed x.LedMode x.LedSpeed x.LedBrightness x.LedColor.R x.LedColor.G x.LedColor.B
            |> ignore
            
        | SetLedSpeed speed ->
            x.LedSpeed <- speed
            x.Bus.SetLed x.LedMode x.LedSpeed x.LedBrightness x.LedColor.R x.LedColor.G x.LedColor.B
            |> ignore
            
        | SetLedBrightness brightness ->
            x.LedBrightness <- brightness
            x.Bus.SetLed x.LedMode x.LedSpeed x.LedBrightness x.LedColor.R x.LedColor.G x.LedColor.B
            |> ignore

    member x.Send(command: EkCommand) =
        if x.CommandQueue.Count > 0 then
            x.CommandQueue.Enqueue command
        else
            x.CommandQueue.Enqueue command

            while x.CommandQueue.Count > 0 do
                let currentCommand = x.CommandQueue.Peek()
                x.ProcessCommand currentCommand
                x.CommandQueue.Dequeue() |> ignore
            
            shouldUpdate <- true