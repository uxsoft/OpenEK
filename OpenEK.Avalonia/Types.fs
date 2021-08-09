module OpenEk.Avalonia.Types

open Elmish
open OpenEK.Core
open OpenEK.Core.EK
open OpenEK.Core.System

type Page =
    | Dashboard = 0
    | Illumination = 1

type ComputeInfo =
    { CpuName: string
      CpuTemperature: float32
      GpuName: string
      GpuTemperature: float32 }
        
let computer = HwInfo.create()
        
let getInfo () =
    { CpuName = HwInfo.getCpuName computer
      CpuTemperature = HwInfo.getCpuTemperature computer "CPU Package"
      GpuName = HwInfo.getGpuName computer
      GpuTemperature = HwInfo.getGpuTemperature computer "GPU Core" }

type Model =
    { CurrentPage: Page
      Compute: ComputeInfo
      IsConnected: bool
      Fans: Map<int, FanData>
      Pump: FanData option
      Lights: LedData option }

type Msg =
    | Navigate of Page
    | UpdateComputeInfo
    | UpdateFans 
    | UpdatePump
    | UpdateLights

let init () : Model * Cmd<Msg> =
    
    let isConnected = EK.Device.connect()
    
    { CurrentPage = Page.Dashboard
      Compute = getInfo()
      IsConnected = isConnected
      Fans = EK.Device.getFans false
      Pump = EK.Device.getPump ()
      Lights = EK.Device.getLed () }, []

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Navigate page -> { model with CurrentPage = page }, []
    | UpdateComputeInfo -> { model with Compute = getInfo() }, []
    | UpdateFans -> { model with Fans = EK.Device.getFans false }, []
    | UpdatePump -> { model with Pump = EK.Device.getPump () }, []
    | UpdateLights -> { model with Lights = EK.Device.getLed () }, []