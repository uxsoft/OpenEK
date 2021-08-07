module OpenEk.Avalonia.IlluminationPage

open Avalonia
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media.Immutable
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
            
            match model.Lights with
            | None -> label { "Failed to retrieve LED data" }
            | Some led -> 
                let color = Color.FromRgb(led.Red, led.Green, led.Blue)
                border {
                    background (ImmutableSolidColorBrush color)
                    label { $"{led.Blue}" }
                }

        }
    }