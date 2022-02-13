module OpenEk.Avalonia.Types

open Elmish
open OpenEk.Core.EK
open OpenEk.Core.System
open FUI.ObservableValue

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
    { Compute: ComputeInfo var
      Device: Commands.DeviceState }

let init () =
    { Compute = var (getInfo())
      Device = Commands.emptyState }
    //TODO start [ fun dispatch -> Event.add (OnDeviceStateChanged >> dispatch) Commands.bus.OnStateChanged ]

//let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
//    match msg with
//    | Navigate page -> { model with CurrentPage = page }, []
//    | UpdateComputeInfo -> { model with Compute = getInfo() }, []
//    | OnDeviceStateChanged deviceState ->
//        { model with Device = deviceState }, []
//    | UpdateFans ->
//        Commands.bus.Send Commands.EkCommand.GetFans
//        model, []
//    | UpdatePump ->
//        Commands.bus.Send Commands.EkCommand.GetPump
//        model, []
//    | UpdateLights ->
//        Commands.bus.Send Commands.EkCommand.GetLed
//        model, []