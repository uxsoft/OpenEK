module OpenEk.Avalonia.Types

open Elmish
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
      Device: Commands.DeviceState }

type Msg =
    | Navigate of Page
    | UpdateComputeInfo
    | OnDeviceStateChanged of Commands.DeviceState
    | UpdateFans 
    | UpdatePump
    | UpdateLights

let init () =
    { CurrentPage = Page.Dashboard
      Compute = getInfo()
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