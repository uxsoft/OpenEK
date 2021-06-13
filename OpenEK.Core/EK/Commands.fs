module OpenEK.Core.EK.Commands

open System.Drawing
open System.Collections.Generic

type EkCommand =
    | SetFansPwm of uint16
    | SetPumpPwm of uint16 
    | SetLedColor of Color
    | SetLedMode of LedMode
    | SetLedSpeed of byte

type DeviceState =
    {
        Pump: FanData
        Fans: Map<int, FanData>
        LedMode: LedMode
        LedColor: Color
        LedSpeed: byte
    }
    
let emptyDeviceState =
    {
        Pump =
            { Model = 0us
              RatedSpeed = 0us
              Speed = 0us
              RatedPower = 0us
              Power = 0us
              Load = 0us
              Pwm = 0us }

        Fans = Map.empty<int, FanData>
        LedMode = LedMode.Off
        LedColor = Color.White
        LedSpeed = 0uy
    }

let createQueue() = Queue<EkCommand>()

let getState () =
    match Device.getLed() with
    | None -> emptyDeviceState
    | Some ledData ->
        {
            Fans = Device.getFans false
            Pump = Device.getPump() |> Option.defaultValue Unchecked.defaultof<FanData>
    
    
            LedMode = ledData.Mode
            LedColor =
                Color.FromArgb(
                    int ledData.Brightness,
                    int ledData.Red,
                    int ledData.Green,
                    int ledData.Blue)
            LedSpeed = ledData.Speed
        }

let sendCommand (state: DeviceState) (command: EkCommand) =
    match command with
    | SetFansPwm pwm ->
        for fan in state.Fans do
            Device.setFan fan.Value (byte fan.Key) pwm
            |> ignore
        state
                    
    | SetPumpPwm pwm ->
        Device.setPump state.Pump pwm |> ignore
        state
        
    | SetLedColor color ->
        Device.setLed state.LedMode state.LedSpeed color.A color.R color.G color.B
        |> ignore
        { state with LedColor = color }
        
    | SetLedMode mode ->
        Device.setLed mode state.LedSpeed state.LedColor.A state.LedColor.R state.LedColor.G state.LedColor.B
        |> ignore
        { state with LedMode = mode }
        
    | SetLedSpeed speed ->
        Device.setLed state.LedMode speed state.LedColor.A state.LedColor.R state.LedColor.G state.LedColor.B
        |> ignore
        { state with LedSpeed = speed }

let queueCommand (state: DeviceState) (onStateUpdated: DeviceState -> unit) (queue: Queue<EkCommand>) (command: EkCommand)  =
    queue.Enqueue command
    
    if queue.Count = 1 then // If count is more than one then an existing loop is already running9
        queue.Enqueue command

        let mutable currentState = state
        
        while queue.Count > 0 do
            let currentCommand = queue.Peek()
            currentState <- sendCommand currentState currentCommand
            onStateUpdated currentState
            queue.Dequeue() |> ignore