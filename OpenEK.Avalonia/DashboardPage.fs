module OpenEk.Avalonia.DashboardPage

open Avalonia
open Avalonia.Layout
open OpenEK.Core.EK
open OpenEk.Avalonia.Types
open Avalonia.FuncUI.Experiments.DSL.DSL

let view (model: Model) dispatch =
    grid {
        margin (Thickness(20.))
        rowDefinitions "50, *"
        label {
            fontSize 24.
            "Dashboard"
        }
        grid {
            row 1
            columnDefinitions "*, *, *, *, *"
            rowDefinitions "*, *, *, *"
            grid {
                column 0
                UI.statistic model.Compute.CpuName $"{model.Compute.CpuTemperature}°C"
            }
            grid {
                column 1
                UI.statistic model.Compute.GpuName $"{model.Compute.GpuTemperature}°C"
            }
            match Commands.bus.State.Pump with
            | None -> ()
            | Some pump ->
                grid {
                    column 2
                    UI.biStatistic "PUMP" $"{pump.Pwm}%%" $"RPM: {pump.Speed}"
                }
            button {
                column 4
                width 32.
                height 32.
                verticalAlignment VerticalAlignment.Top
                horizontalAlignment HorizontalAlignment.Right
                margin (Thickness(20.))
                onClick (fun _ ->
                    dispatch UpdateComputeInfo
                    dispatch UpdateFans
                    dispatch UpdatePump)
                UI.refreshSymbol
            }
            
            for fan in Commands.bus.State.Fans do
                border {
                    row 1
                    column fan.Key
                    UI.biStatistic $"FAN{fan.Key}" $"{fan.Value.Pwm}%%" $"RPM: {fan.Value.Speed}"
                }
                
            border {
                row 2
                label { "charts here" }
            }
            
            match Commands.bus.State.Fans |> Seq.tryHead with
            | None -> label { () }
            | Some fan -> 
                stackPanel {
                    column 0
                    row 3
                    UI.headerLabel "Fans"
                    label { $"{fan.Value.Pwm}%%" }
                    slider {
                        margin (Thickness 2.)
                        minimum (float 0)
                        maximum (float 100)
                        value (float fan.Value.Pwm)
                        onValueChanged (uint16 >> Commands.EkCommand.SetFansPwm >> Commands.bus.Send) 
                    }
                }

            if Commands.bus.State.Pump.IsSome then
                stackPanel {
                    column 1
                    row 3
                    UI.headerLabel "Water Pump"
                    let pump = Commands.bus.State.Pump
                    
                    label { $"{pump.Value.Pwm}%%" }
                    slider {
                        margin (Thickness 2.)
                        //minimum (float 0)
                        maximum (float 100)
                        value (float pump.Value.Pwm)
                        onValueChanged (uint16 >> Commands.EkCommand.SetPumpPwm >> Commands.bus.Send) 
                    }
                }
                
            stackPanel {
                column 3
                row 3
                UI.headerLabel "AutoPilot"
                checkBox {
                    isChecked true
                }
            }
        }
    }