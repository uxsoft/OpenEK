module OpenEk.Avalonia.UI

open Avalonia
open Avalonia.FuncUI.Experiments.DSL.DSL
open Avalonia.Media
open Avalonia.Media.Immutable

let statistic title value =
    stackPanel {
        margin (Thickness(20.))
        text {
            text title
        }
        text {
            row 1
            fontSize 24.
            text value
        }
    }
    
let biStatistic title value subTitle =
    stackPanel {
        margin (Thickness(20.))
        text {
            text title
        }
        text {
            fontSize 24.
            text value
        }
        text {
            foreground (ImmutableSolidColorBrush Colors.Gray)
            text subTitle
        }
    }
    
let symbol hexCode =
    text {
        fontFamily (FontFamily.Parse("Segoe MDL2 Assets"))
        text $"{char hexCode}"
    }
    
let refreshSymbol = symbol 0xE72C