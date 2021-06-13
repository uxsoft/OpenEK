module OpenEK.Core.EKService

open System.Timers
open OpenEK.Core.EK
open OpenEK.Core.System

let onDataUpdatedEvent = Event<unit>()
let onConnectedEvent = Event<unit>()

[<CLIEvent>]
let OnDataUpdated = onDataUpdatedEvent.Publish

[<CLIEvent>]
let OnConnected = onConnectedEvent.Publish

let cpuSensor = "CPU Package"
let gpuSensor = "GPU Core"
let timer = new Timer(1000., AutoReset = true)
let computer = HwInfo.create()
let mutable temps = HwTemps.empty

let update () =
    HwInfo.update computer
    let cpu = HwInfo.getCpuTemperature computer cpuSensor
    let gpu = HwInfo.getGpuTemperature computer gpuSensor
    
    temps <- HwTemps.tick 6 (float cpu) (float gpu) temps 
        
let onTimerElapsed e =
    update ()
       
let cpu () = temps.Cpu |> List.tryLast |> Option.defaultValue -1.
let effectiveCpu () = temps.CpuAdjusted |> List.tryLast |> Option.defaultValue -1.
let gpu () = temps.Gpu |> List.tryLast |> Option.defaultValue -1.
let effectiveGpu () = temps.GpuAdjusted |> List.tryLast |> Option.defaultValue -1.

// Commands
let mutable deviceState = Commands.getState()

let commandQueue = Commands.createQueue()

let queueCommand =
    Commands.queueCommand deviceState (fun s -> deviceState <- s) commandQueue

let refreshState () = deviceState <- Commands.getState()

let connect () =
    if Device.reconnect() = false then
        failwith "Unable to connect to the EK Connect Hardware"

    timer.Start()

let disconnect () =
    Device.disconnect()
    timer.Stop()

do       
    timer.Elapsed.Add onTimerElapsed