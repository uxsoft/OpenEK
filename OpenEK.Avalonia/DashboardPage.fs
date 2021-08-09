module OpenEk.Avalonia.DashboardPage

open Avalonia
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media
open Avalonia.Media.Immutable
open OpenEk.Avalonia.Types
open Avalonia.FuncUI.Experiments.DSL.DSL



let view (model: Model) dispatch =
    grid {
        rowDefinitions "50, *"
        label {
            margin (Thickness(20.))
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
            match model.Pump with
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
            
            for fan in model.Fans do
                border {
                    row 1
                    column fan.Key
                    UI.biStatistic $"FAN{fan.Key}" $"{fan.Value.Pwm}%%" $"RPM: {fan.Value.Speed}"
                }
                
            border {
                row 2
                label { "charts here" }
            }
            
            border {
                row 3
                label { "controls here" }
            }
        }
    }