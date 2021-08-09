module OpenEk.Avalonia.IlluminationPage

open System
open Avalonia
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media.Immutable
open OpenEK.Core.EK
open OpenEk.Avalonia.Types
open Avalonia.FuncUI.Experiments.DSL.DSL

let view (model: Model) dispatch =
    grid {
        rowDefinitions "48, *"
        label {
            margin (Thickness(20.))
            fontSize 24.
            "Illumination"
        }
        
        button {
            row 1
            column 4
            width 32.
            height 32.
            verticalAlignment VerticalAlignment.Top
            horizontalAlignment HorizontalAlignment.Right
            margin (Thickness(20.))
            onClick (fun _ -> dispatch UpdateLights)
            UI.refreshSymbol
        }
        
        stackPanel {
            row 1
            margin (Thickness(20.))
            
            match Commands.bus.State.Led with
            | None -> label { "Failed to retrieve LED data" }
            | Some led -> 
                let color = Color.FromRgb(led.Red, led.Green, led.Blue)
                
                UI.headerLabel "Mode"
                comboBox {
                    dataItems (Enum.GetNames<LedMode>())
                    selectedItem (string led.Mode)
                }
                
                UI.headerLabel "Color" 
                border {
                    horizontalAlignment HorizontalAlignment.Left
                    background (ImmutableSolidColorBrush color)
                    width 48.
                    height 48.
                }
                
                UI.headerLabel "Brightness" 
                numberInput { value (double led.Brightness) }
                
                UI.headerLabel "Speed" 
                numberInput { value (double led.Speed) }
        }
    }