module OpenEk.Avalonia.IlluminationPage

open Avalonia
open OpenEk.Avalonia.Types
open Avalonia.FuncUI.Experiments.DSL.DSL

let view (model: Model) dispatch =
    grid {
        text {
            margin (Thickness(20.))
            fontSize 24.
            text "Illumination"
        }
    }