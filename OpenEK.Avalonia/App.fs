module OpenEk.Avalonia.App

open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open FUI.Avalonia.DSL
open OpenEK.Core.EK
open OpenEk.Avalonia
open Types

let view (model: Model) =
    Grid {
        DockPanel {
            AcrylicBorder {
                isHitTestVisible false
                material (ExperimentalAcrylicMaterial(TintColor = Colors.Black, MaterialOpacity = 0.80, TintOpacity = 1.))
                dock Dock.Left
                width 240.
            }
            AcrylicBorder {
                isHitTestVisible false
                material (ExperimentalAcrylicMaterial(TintColor = Color.Parse("#222222"), MaterialOpacity = 0.80, TintOpacity = 1.))
            }
        }
        
        TabControl {
            classes ["sidebar"]

            TabItem {
                header "Dashboard"
                DashboardPage.view model
            }
            TabItem {
                header "Illumination"
                IlluminationPage.view model
            }
        }

        StackPanel {
            orientation Orientation.Horizontal
            margin (Thickness(6.))
            
            Border {
                margin (Thickness(0., 2., 4., 0.))
                
                match Commands.bus.State.IsConnected with
                | true -> UI.circleOnSymbol()
                | false -> UI.circleEmptySymbol()
            }
            Label { "EK Connect" }
        }
    }