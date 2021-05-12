module OpenEK.Core.HwInfo.HardwareMonitor

open System
open System.Collections.Generic
open System.Linq
open LibreHardwareMonitor.Hardware

let mutable computer = Computer(IsCpuEnabled = true, IsGpuEnabled = true)
do computer.Open()

let update () =
    for hardware in computer.Hardware do
        hardware.Update()

let getCpu() =
    computer.Hardware.SingleOrDefault(fun hw -> hw.HardwareType = HardwareType.Cpu)

let getCpuName() =
    getCpu().Name

let getCpuTemperatureSensors() =
    let cpu = getCpu()
    cpu.Sensors.Select(fun s -> s.Name) // ?? Array.Empty<string>();

let getCpuTemperature sensorName =
    let cpu = getCpu()
    let sensor =
        cpu.Sensors.SingleOrDefault(fun s -> 
            s.SensorType = SensorType.Temperature && 
            s.Name = sensorName);
    
    sensor.Value// ?? 0;

let getGpu() =
    computer.Hardware.SingleOrDefault(fun hw -> 
        hw.HardwareType = HardwareType.GpuAmd || hw.HardwareType = HardwareType.GpuNvidia);

let getGpuName() =
    getGpu().Name

let getGpuTemperatureSensors() =
    let gpu = getGpu()
    gpu.Sensors.Select(fun s -> s.Name)

let getGpuTemperature sensorName =
    let gpu = getGpu()
    let sensor = gpu.Sensors.SingleOrDefault(fun s -> 
        s.SensorType = SensorType.Temperature && 
        s.Name = sensorName)
    sensor.Value
