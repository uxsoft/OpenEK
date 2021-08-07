module OpenEk.Avalonia.UI

open Avalonia
open Avalonia.FuncUI.Experiments.DSL.DSL
open Avalonia.Media
open Avalonia.Media.Immutable

let statistic title value =
    stackPanel {
        margin (Thickness(20.))
        label { text title }
        label {
            row 1
            fontSize 24.
            text value
        }
    }
    
let biStatistic title value subTitle =
    stackPanel {
        margin (Thickness(20.))
        label { text title }
        label {
            fontSize 24.
            text value
        }
        label {
            foreground (ImmutableSolidColorBrush Colors.Gray)
            text subTitle
        }
    }
    
let symbol hexCode (color: Color) =
    label {
        fontFamily (FontFamily.Parse("Segoe MDL2 Assets"))
        foreground (ImmutableSolidColorBrush color)
        $"{char hexCode}"
    }
    
let lightbulbOffSymbol = symbol 0xEA80
let lightbulbOnSymbol = symbol 0xEA80

let circleErrorSymbol = symbol 0xEA39 Colors.Red
let circleEmptySymbol = symbol 0xEA3A Colors.White
let circleOnSymbol = symbol 0xEA3B Colors.Green

let refreshSymbol = symbol 0xE72C Colors.White