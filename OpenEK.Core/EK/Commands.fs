module OpenEk.Core.EK.Commands

open FUI.ObservableValue

type EkCommand =
    | GetFans
    | GetPump
    | GetLed
    | SetFansPwm of uint16
    | SetPumpPwm of uint16 
    | SetLedColor of byte * byte * byte
    | SetLedMode of LedMode
    | SetLedSpeed of byte

type DeviceState =
    { IsConnected: bool var
      Pump: FanData option var
      Fans: Map<int, FanData> var
      Led: LedData option var }
    
let emptyState =
    { IsConnected = var false
      Pump = var None
      Fans = var Map.empty<int, FanData>
      Led  = var None }

let getState () =
    { IsConnected = var (Device.reconnect ())
      Fans = var (Device.getFans false)
      Pump = var (Device.getPump())
      Led = var (Device.getLed()) }

let sendCommand (command: EkCommand) (state: DeviceState) =
    match command with
    | GetFans ->
         let fans = Device.getFans false
         state.Fans.Value <- fans
         
    | GetPump ->
        let pump = Device.getPump ()
        state.Pump.Value <- pump
        
    | GetLed ->
        let ledData = Device.getLed ()
        state.Led.Value <- ledData
        
    | SetFansPwm pwm ->
        for fan in state.Fans.Value do
            Device.setFan fan.Value (byte fan.Key) pwm
            |> ignore
        let fans = state.Fans.Value |> Map.map (fun _ fan  -> { fan with Pwm = pwm })
        state.Fans.Value <- fans        
                    
    | SetPumpPwm pwm ->
        match state.Pump.Value with
        | None -> ()
        | Some pump -> 
            Device.setPump pump pwm |> ignore
            let pump = Some { pump with Pwm = pwm }
            state.Pump.Value <- pump
        
    | SetLedColor (r, g, b) ->
        match state.Led.Value with
        | None -> ()
        | Some led ->
            Device.setLed led.Mode led.Speed led.Brightness r g b |> ignore
            let led = Some { led with Red = r; Green = g; Blue = b }
            state.Led.Value <- led
        
    | SetLedMode mode ->
        match state.Led.Value with
        | None -> ()
        | Some led ->
            Device.setLed mode led.Speed led.Brightness led.Red led.Green led.Blue |> ignore
            let led = Some { led with Mode = mode }
            state.Led.Value <- led
        
    | SetLedSpeed speed ->
        match state.Led.Value with
        | None -> ()
        | Some led ->
            Device.setLed led.Mode speed led.Brightness led.Red led.Green led.Blue |> ignore
            let led = Some { led with Speed = speed }
            state.Led.Value <- led
            
type EkConnectBus() =
    
    let state = getState()
    
    let agent = 
        new MailboxProcessor<EkCommand>(fun inbox ->
            async {
                while true do
                    let! message = inbox.Receive()
                    sendCommand message state
            })
    
    do agent.Start()
    
    member _.State = state
    member _.Send msg = agent.Post msg
    
let bus = EkConnectBus()