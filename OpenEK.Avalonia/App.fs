module OpenEK.Avalonia.App

open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open Avalonia.FuncUI.Experiments.DSL.DSL
open OpenEk.Avalonia
open Types

let view (model: Model) dispatch =
    grid {
        dockPanel {
            acrylicBorder {
                isHitTestVisible false
                material (ExperimentalAcrylicMaterial(TintColor = Colors.Black, MaterialOpacity = 0.85, TintOpacity = 1.))
                dock Dock.Left
                width 240.
            }
            acrylicBorder {
                isHitTestVisible false
                material (ExperimentalAcrylicMaterial(TintColor = Color.Parse("#222222"), MaterialOpacity = 0.85, TintOpacity = 1.))
            }
        }
        
        tabControl {
            classes ["sidebar"]
            selectedIndex (int model.CurrentPage)
            
            tabItem {
                header "Dashboard"
                onTapped (fun _ -> dispatch (Navigate Page.Dashboard))
                DashboardPage.view model dispatch
            }
            tabItem {
                header "Illumination"
                onTapped (fun _ -> dispatch (Navigate Page.Illumination))
                IlluminationPage.view model dispatch
            }
        }

        stackPanel {
            orientation Orientation.Horizontal
            margin (Thickness(6.))
            
            border {
                margin (Thickness(0., 2., 4., 0.))
                
                match model.IsConnected with
                | true -> UI.circleOnSymbol
                | false -> UI.circleEmptySymbol
            }
            label {
                
                "EK Connect"
            }
        }
    }