module OpenEK.Core.EK.Commands

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
    { IsConnected: bool
      Pump: FanData option
      Fans: Map<int, FanData>
      Led: LedData option }
    
let emptyState =
    { IsConnected = false
      Pump = None
      Fans = Map.empty<int, FanData>
      Led  = None }

let getState () =
    { IsConnected = Device.reconnect ()
      Fans = Device.getFans false
      Pump = Device.getPump()
      Led = Device.getLed() }

let sendCommand (state: DeviceState) (command: EkCommand) =
    match command with
    | GetFans ->
         let fans = Device.getFans false
         { state with Fans = fans }
         
    | GetPump ->
        let pump = Device.getPump ()
        { state with Pump = pump }
        
    | GetLed ->
        let ledData = Device.getLed ()
        { state with Led = ledData }
        
    | SetFansPwm pwm ->
        for fan in state.Fans do
            Device.setFan fan.Value (byte fan.Key) pwm
            |> ignore
        { state with Fans = state.Fans |> Map.map (fun _ fan  -> { fan with Pwm = pwm }) }
                    
    | SetPumpPwm pwm ->
        match state.Pump with
        | None -> state
        | Some pump -> 
            Device.setPump pump pwm |> ignore
            { state with Pump = Some { pump with Pwm = pwm } }
        
    | SetLedColor (r, g, b) ->
        match state.Led with
        | None -> state
        | Some led ->
            Device.setLed led.Mode led.Speed led.Brightness r g b |> ignore
            { state with Led = Some { led with Red = r; Green = g; Blue = b } }
        
    | SetLedMode mode ->
        match state.Led with
        | None -> state
        | Some led ->
            Device.setLed mode led.Speed led.Brightness led.Red led.Green led.Blue |> ignore
            { state with Led = Some { led with Mode = mode } }
        
    | SetLedSpeed speed ->
        match state.Led with
        | None -> state
        | Some led ->
            Device.setLed led.Mode speed led.Brightness led.Red led.Green led.Blue |> ignore
            { state with Led = Some { led with Speed = speed } }
            
type EkConnectBus() =
    
    let onStateChanged = Event<DeviceState>()
    let mutable state = getState()
    
    let agent = 
        new MailboxProcessor<EkCommand>(fun inbox ->
            async {
                while true do
                    let! message = inbox.Receive()
                    state <- sendCommand state message
                    onStateChanged.Trigger state
            })
        
    member _.State = state
    member _.OnStateChanged = onStateChanged.Publish
    member _.Send msg = agent.Post msg
    
let bus = EkConnectBus()