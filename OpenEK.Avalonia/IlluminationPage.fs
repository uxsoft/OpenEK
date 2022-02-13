module OpenEk.Avalonia.IlluminationPage

open System
open Avalonia
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media.Immutable
open FUI
open OpenEk.Core.EK
open OpenEk.Avalonia.Types
open FUI.Avalonia.DSL
open FUI.FragmentBuilder

let LedForm led =
    Fragment {
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
            onClick (fun _ -> Commands.bus.Send Commands.EkCommand.GetLed)
            UI.refreshSymbol()
        }
        
        StackPanel {
            row 1
            margin (Thickness(20.))
            
            for ledOption in Commands.bus.State.Led |> Ov.toObservableCollection do
                match ledOption with
                | None -> Fragment { Label { "Failed to retrieve LED data" } }
                | Some led -> LedForm led
        }
    }