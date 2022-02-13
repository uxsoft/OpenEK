module OpenEk.Avalonia.DashboardPage

open Avalonia
open Avalonia.Layout
open FUI
open OpenEk.Core.EK
open OpenEk.Avalonia.Types
open FUI.Avalonia.DSL
open FUI.FragmentBuilder

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
                
                for compute in model.Compute |> Ov.toObservableCollection do
                    UI.statistic compute.CpuName $"{compute.CpuTemperature}°C"
            }
            Grid {
                column 1
                
                for compute in model.Compute |> Ov.toObservableCollection do
                    UI.statistic compute.GpuName $"{compute.GpuTemperature}°C"
            }
            
            for pumpOption in Commands.bus.State.Pump |> Ov.toObservableCollection do
                match pumpOption with
                | None -> Grid { () }
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
                onClick (fun _ ->
                    model.Compute.Update (ignore >> getInfo)
                    Commands.bus.Send Commands.EkCommand.GetFans
                    Commands.bus.Send Commands.EkCommand.GetPump)
                UI.refreshSymbol()
            }
            
            for fanMap in Commands.bus.State.Fans |> Ov.toObservableCollection do
                for fan in fanMap do
                    Border {
                        row 1
                        column fan.Key
                        UI.biStatistic $"FAN{fan.Key}" $"{fan.Value.Pwm}%%" $"RPM: {fan.Value.Speed}"
                    }
                
            Border {
                row 2
                Label { "charts here" }
            }
            
            for fanMap in Commands.bus.State.Fans |> Ov.toObservableCollection do
                match fanMap |> Seq.tryHead with
                | None -> Fragment { () }
                | Some fan ->
                    StackPanel {
                        column 0
                        row 3
                        UI.headerLabel "Fans"
                        Label { $"{fan.Value.Pwm}%%" }
                        Slider {
                            margin (Thickness 2.)
                            maximum (float 100)
                            value (float fan.Value.Pwm)
                            onValueChanged (uint16 >> Commands.EkCommand.SetFansPwm >> Commands.bus.Send) 
                        }
                    }

            for pumpOption in Commands.bus.State.Pump |> Ov.toObservableCollection do
                match pumpOption with            
                | None -> Fragment { () }
                | Some pump ->
                    Fragment { 
                        StackPanel {
                            column 1
                            row 3
                            UI.headerLabel "Water Pump"
                            
                            Label { $"{pump.Pwm}%%" }
                            Slider {
                                margin (Thickness 2.)
                                maximum (float 100)
                                value (float pump.Pwm)
                                onValueChanged (uint16 >> Commands.EkCommand.SetPumpPwm >> Commands.bus.Send) 
                            }
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