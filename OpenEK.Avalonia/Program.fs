module FuncUI.Experiments.Program

open System.Text.Json
open Avalonia.Controls
open Avalonia.Media
open Avalonia.Media.Immutable
open Avalonia.Platform
open Avalonia.Styling
open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts
open OpenEK.Avalonia
open Live.Avalonia
open OpenEk.Avalonia.Types

let transferState<'t> oldState =
    try
        let json = Thoth.Json.Encode.Auto.toString(4, oldState)
        let state = Thoth.Json.Decode.Auto.fromString<'t>(json)        
        match state with
        | Error err -> printfn $"{err}"; None
        | Ok model -> Some (model, [])
    with _ -> None

let isProduction () =
    #if false // DEBUG
        false
    #else
        true
    #endif
    
type MainControl(parent: Window) as this =
    inherit HostControl()
    do
        OpenEK.Core.EK.Device.disconnect()
        
        let hotInit () =
            match transferState<Model> parent.DataContext with
            | Some newState -> newState
            | None -> init ()
        
        Elmish.Program.mkProgram hotInit update App.view
        |> Program.withHost this
        |> Program.withTrace (fun msg model ->
            printfn $"Message: {msg}"
            printfn $"Model: {model}"
            parent.DataContext <- model)
        |> Program.run

type App() =
    inherit Application()
    
    interface ILiveView with
        member _.CreateView(window: Window) =
            MainControl(window) :> obj

    override this.Initialize() =
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"
        this.Styles.Load "avares://OpenEk.Avalonia/Styles/SideBar.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            let window =
                if isProduction() then
                    let window = new LiveViewHost(this, fun msg -> printfn $"%s{msg}")
                    window.StartWatchingSourceFilesForHotReloading()
                    window :> Window
                else
                    let window = Window()
                    window.Content <- MainControl(window)
                    window                    
            
            window.Title <- "Open EK Connect"
            window.TransparencyLevelHint <- WindowTransparencyLevel.AcrylicBlur
            window.SystemDecorations <- SystemDecorations.Full
            window.ExtendClientAreaToDecorationsHint <- true
            window.ExtendClientAreaTitleBarHeightHint <- -1.
            window.ExtendClientAreaChromeHints <- ExtendClientAreaChromeHints.PreferSystemChrome
            window.Background <- null
            window.Show()
            
            base.OnFrameworkInitializationCompleted()
        | _ -> ()

[<EntryPoint>]
let main(args: string[]) =
    AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .UseSkia()
        .With(SkiaOptions(MaxGpuResourceSizeBytes = 8096000L) :> obj)
        .With(Win32PlatformOptions(AllowEglInitialization = true))
        .StartWithClassicDesktopLifetime(args)