module OpenEK.Core.Fans

open System

let stepCurve tAmbient tMax tNow =
    match tNow with
    | t when t <= 30 -> 0
    | t when t <= 40 -> 5
    | t when t <= 50 -> 10
    | t when t <= 60 -> 20
    | t when t <= 70 -> 40
    | t when t <= 80 -> 80
    | _ -> 99
    
let defaultStepCurve = 
    stepCurve 30 80 

let linearCurve tAmbient tMax tNow = 
    let pwm = Math.Max(0, tNow - tAmbient) / (tMax - tAmbient)
    Math.Min(pwm, 99)
    
let defaultLinearCurve =
    linearCurve 30 80