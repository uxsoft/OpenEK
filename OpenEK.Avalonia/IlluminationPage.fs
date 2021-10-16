module OpenEk.Avalonia.IlluminationPage

open System
open Avalonia
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media.Immutable
open OpenEK.Core.EK
open OpenEk.Avalonia.Types
open FUI.Avalonia.DSL

let view (model: Model) =
    Grid {
        rowDefinitions "48, *"
        Label {
            margin (Thickness(20.))
            fontSize 24.
            "Illumination"
        }
        
        Button {
            row 1
            column 4
            width 32.
            height 32.
            verticalAlignment VerticalAlignment.Top
            horizontalAlignment HorizontalAlignment.Right
            margin (Thickness(20.))
//TODO update for FUI
//            onClick (fun _ -> dispatch UpdateLights)
            UI.refreshSymbol
        }
        
        StackPanel {
            row 1
            margin (Thickness(20.))
            
            match Commands.bus.State.Led with
            | None -> Label { "Failed to retrieve LED data" }
            | Some led -> 
                let color = Color.FromRgb(led.Red, led.Green, led.Blue)
                
                UI.headerLabel "Mode"
                ComboBox {
                    items (Enum.GetNames<LedMode>())
                    selectedItem (string led.Mode)
                }
                
                UI.headerLabel "Color" 
                Border {
                    horizontalAlignment HorizontalAlignment.Left
                    background (ImmutableSolidColorBrush color)
                    width 48.
                    height 48.
                }
                
                UI.headerLabel "Brightness" 
                NumberInput { value (double led.Brightness) }
                
                UI.headerLabel "Speed" 
                NumberInput { value (double led.Speed) }
        }
    }