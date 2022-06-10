module OpenEk.Core.System.HwInfo

open System.Linq
open LibreHardwareMonitor.Hardware

let create() =
    let computer = Computer(IsCpuEnabled = true, IsGpuEnabled = true)
    computer.Open()
    computer
    
let update (computer: Computer) =
    for hardware in computer.Hardware do
        hardware.Update()

let getCpu (computer: Computer) =
    computer.Hardware.SingleOrDefault(fun hw -> hw.HardwareType = HardwareType.Cpu)

let getCpuName (computer: Computer) =
    getCpu(computer).Name

let getCpuTemperatureSensors (computer: Computer) =
    let cpu = getCpu computer
    cpu.Sensors.Select(fun s -> s.Name)

let getCpuTemperature (computer: Computer) sensorName =
    let cpu = getCpu computer
    let sensor =
        cpu.Sensors.SingleOrDefault(fun s -> 
            s.SensorType = SensorType.Temperature && 
            s.Name = sensorName);
    
    sensor
        |> Option.ofObj
        |> Option.bind (fun s -> Option.ofNullable s.Value)
        |> Option.defaultValue 0.f

let getGpu (computer: Computer) =
    computer.Hardware.SingleOrDefault(fun hw -> 
        hw.HardwareType = HardwareType.GpuAmd || hw.HardwareType = HardwareType.GpuNvidia);

let getGpuName (computer: Computer) =
    getGpu(computer).Name.Replace("NVIDIA NVIDIA", "NVIDIA")

let getGpuTemperatureSensors (computer: Computer) =
    let gpu = getGpu computer
    gpu.Sensors.Select(fun s -> s.Name)

let getGpuTemperature (computer: Computer) sensorName =
    let gpu = getGpu computer
    let sensor = gpu.Sensors.SingleOrDefault(fun s -> 
        s.SensorType = SensorType.Temperature && 
        s.Name = sensorName)
    
    sensor
        |> Option.ofObj
        |> Option.bind (fun s -> Option.ofNullable s.Value)
        |> Option.defaultValue 0.f