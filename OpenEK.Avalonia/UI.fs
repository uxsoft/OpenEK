module OpenEk.Avalonia.UI

open Avalonia
open FUI.Avalonia.DSL
open Avalonia.Media
open Avalonia.Media.Immutable

let headerLabel title =
    Label {
        margin (Thickness(0., 12., 0., 2.))
        text title
    }

let statistic title value =
    StackPanel {
        Label { text title }
        Label {
            row 1
            fontSize 24.
            text value
        }
    }
    
let biStatistic title value subTitle =
    StackPanel {
        Label {
            fontSize 14.
            text title
        }
        Label {
            fontSize 24.
            text value
        }
        Label {
            fontSize 12.
            foreground (ImmutableSolidColorBrush Colors.Gray)
            text subTitle
        }
    }
    
let symbol hexCode (color: Color) =
    Label {
        fontFamily (FontFamily.Parse("Segoe MDL2 Assets"))
        foreground (ImmutableSolidColorBrush color)
        $"{char hexCode}"
    }
    
let lightbulbOffSymbol() = symbol 0xEA80
let lightbulbOnSymbol() = symbol 0xEA80

let circleErrorSymbol() = symbol 0xEA39 Colors.Red
let circleEmptySymbol() = symbol 0xEA3A Colors.White
let circleOnSymbol() = symbol 0xEA3B Colors.Green

let refreshSymbol() = symbol 0xE72C Colors.White