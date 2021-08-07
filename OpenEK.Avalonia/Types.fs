module OpenEk.Avalonia.Types

open Elmish
open OpenEK.Core
open OpenEK.Core.EK
open OpenEK.Core.System

type Page =
    | Dashboard
    | Illumination

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
    
type Device =
    { Pump: int
      Fans: int list }

type Model =
    { CurrentPage: Page
      Compute: ComputeInfo
      Fans: Map<int, FanData>
      Pump: FanData option }

type Msg =
    | UpdateComputeInfo
    | UpdateFans 
    | UpdatePump

let init () : Model * Cmd<Msg> =
//    async { EKManager.connect() } |> Async.Start
    let isConnected = EK.Device.connect()
    
    { CurrentPage = Dashboard
      Compute = getInfo()
      Fans = EK.Device.getFans false
      Pump = EK.Device.getPump () }, []



let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | UpdateComputeInfo -> { model with Compute = getInfo() }, []
    | UpdateFans -> { model with Fans = EK.Device.getFans false }, []
    | UpdatePump -> { model with Pump = EK.Device.getPump () }, []