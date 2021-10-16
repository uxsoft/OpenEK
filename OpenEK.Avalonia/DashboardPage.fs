module OpenEk.Avalonia.DashboardPage

open Avalonia
open Avalonia.Layout
open OpenEK.Core.EK
open OpenEk.Avalonia.Types
open FUI.Avalonia.DSL

let view (model: Model) =
    Grid {
        margin (Thickness(20.))
        rowDefinitions "50, *"
        Label {
            fontSize 24.
            "Dashboard"
        }
        Grid {
            row 1
            columnDefinitions "*, *, *, *, *"
            rowDefinitions "*, *, *, *"
            Grid {
                column 0
                UI.statistic model.Compute.CpuName $"{model.Compute.CpuTemperature}°C"
            }
            Grid {
                column 1
                UI.statistic model.Compute.GpuName $"{model.Compute.GpuTemperature}°C"
            }
            match Commands.bus.State.Pump with
            | None -> ()
            | Some pump ->
                Grid {
                    column 2
                    UI.biStatistic "PUMP" $"{pump.Pwm}%%" $"RPM: {pump.Speed}"
                }
            Button {
                column 4
                width 32.
                height 32.
                verticalAlignment VerticalAlignment.Top
                horizontalAlignment HorizontalAlignment.Right
                margin (Thickness(20.))
// TODO update for FUI
//                onClick (fun _ ->
//                    dispatch UpdateComputeInfo
//                    dispatch UpdateFans
//                    dispatch UpdatePump)
                UI.refreshSymbol
            }
            
            for fan in Commands.bus.State.Fans do
                Border {
                    row 1
                    column fan.Key
                    UI.biStatistic $"FAN{fan.Key}" $"{fan.Value.Pwm}%%" $"RPM: {fan.Value.Speed}"
                }
                
            Border {
                row 2
                Label { "charts here" }
            }
            
            match Commands.bus.State.Fans |> Seq.tryHead with
            | None -> Label { () }
            | Some fan -> 
                StackPanel {
                    column 0
                    row 3
                    UI.headerLabel "Fans"
                    Label { $"{fan.Value.Pwm}%%" }
                    Slider {
                        margin (Thickness 2.)
                        minimum (float 0)
                        maximum (float 100)
                        value (float fan.Value.Pwm)
                        onValueChanged (uint16 >> Commands.EkCommand.SetFansPwm >> Commands.bus.Send) 
                    }
                }

            if Commands.bus.State.Pump.IsSome then
                StackPanel {
                    column 1
                    row 3
                    UI.headerLabel "Water Pump"
                    let pump = Commands.bus.State.Pump
                    
                    Label { $"{pump.Value.Pwm}%%" }
                    Slider {
                        margin (Thickness 2.)
                        //minimum (float 0)
                        maximum (float 100)
                        value (float pump.Value.Pwm)
                        onValueChanged (uint16 >> Commands.EkCommand.SetPumpPwm >> Commands.bus.Send) 
                    }
                }
                
            StackPanel {
                column 3
                row 3
                UI.headerLabel "AutoPilot"
                CheckBox {
                    isChecked true
                }
            }
        }
    }