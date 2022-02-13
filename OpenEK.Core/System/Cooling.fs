module OpenEk.Core.System.Cooling

open System
open OpenEk.Core.EK.Commands

let stepCurve (tAmbient: float) (tMax: float) (tNow: float) : uint16 =
    match tNow with
    | t when t <= 30. -> 0us
    | t when t <= 40. -> 5us
    | t when t <= 50. -> 10us
    | t when t <= 60. -> 20us
    | t when t <= 70. -> 40us
    | t when t <= 80. -> 80us
    | _ -> 99us
    
let defaultStepCurve = 
    stepCurve 30. 80. 

let linearCurve (tAmbient: float) (tMax: float) (tNow: float) : uint16 = 
    let pwm = Math.Max(0., tNow - tAmbient) / (tMax - tAmbient)
    uint16 (Math.Min(pwm, 99.))
    
let defaultLinearCurve =
    linearCurve 30. 80.

let recommendAdjustments (tNow: float) fanPwm pumpPwm =
    let recommendedPwm = defaultStepCurve tNow
    if recommendedPwm <> fanPwm then    
        [ EkCommand.SetFansPwm recommendedPwm;
          EkCommand.SetPumpPwm recommendedPwm]
    else []