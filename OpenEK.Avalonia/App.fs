module OpenEK.Avalonia.App

open Avalonia.Controls
open Avalonia.Media
open Avalonia.FuncUI.Experiments.DSL.DSL
open OpenEk.Avalonia
open Types


let view (model: Model) dispatch =
    grid {
        dockPanel {
            acrylicBorder {
                material (ExperimentalAcrylicMaterial(TintColor = Colors.Black, MaterialOpacity = 0.85, TintOpacity = 1.))
                dock Dock.Left
                width 240.
                isHitTestVisible false
            }
            acrylicBorder {
                isHitTestVisible false
                material (ExperimentalAcrylicMaterial(TintColor = Color.Parse("#222222"), MaterialOpacity = 0.85, TintOpacity = 1.))
            }
        }
        
        tabControl {
            classes ["sidebar"]
            columnSpan 2
            row 1
            
            tabItem {
                header "Dashboard"
                DashboardPage.view model dispatch
            }
            tabItem {
                header "Lightning"
                IlluminationPage.view model dispatch
            }
        }
    }