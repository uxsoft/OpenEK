module OpenEk.Core.System.HwTemps

open System
open FSharp.Stats
open FSharp.Stats.Fitting.LinearRegression

type TempInfo =
    { Cpu: float list
      CpuAdjusted: float list
      Gpu: float list
      GpuAdjusted: float list }

let empty =
    { Cpu = []
      CpuAdjusted = []
      Gpu = []
      GpuAdjusted = [] }

let hysteresis (values: float list) =
    if values.Length <= 1 then
        values |> List.tryHead |> Option.defaultValue 0.
    else
        let count = values |> List.length |> float
    
        let x = vector [| 1. .. count |]
        let y = Vector.ofList values
        
        let coefficients =
            RobustRegression.Linear.theilEstimator x y
    
        let estimate =
            RobustRegression.Linear.fit coefficients (Math.Ceiling(count / 2.))
    
        estimate

let tick count cpu gpu (x: TempInfo) =
    { Cpu = x.Cpu |> List.roll count cpu
      Gpu = x.Gpu |> List.roll count gpu
      CpuAdjusted = x.CpuAdjusted |> List.roll count (hysteresis (x.Cpu @ [cpu]))
      GpuAdjusted = x.GpuAdjusted |> List.roll count (hysteresis (x.Gpu @ [gpu])) }
