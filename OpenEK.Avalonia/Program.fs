module OpenEk.Avalonia.Program

open System
open System.Text.Json
open System.Text.Json.Serialization
open Avalonia.Controls
open Avalonia.Platform
open Avalonia.Themes.Fluent
open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts
open OpenEK.Core
open Live.Avalonia
open OpenEk.Avalonia.Types


let transferModel<'t> previousModel =
    try
        let options = JsonSerializerOptions()
        options.Converters.Add(JsonFSharpConverter())
        
        let json = JsonSerializer.Serialize(previousModel, options)
        let model = JsonSerializer.Deserialize<Model>(json, options)
        
        match box model with
        | null -> failwith "Failed to transfer model"
        | _ ->
            printf $"Successfully transferred model: {model}"
            Some (model, [])
    with err ->
        printfn $"{err}"
        None

let isProduction () =
    #if false // DEBUG
        false
    #else
        true
    #endif
    
type MainControl(parent: Window) as this =
    inherit HostControl()
    do
        EK.Device.disconnect()
        
        let hotInit () =
            match transferModel<Model> parent.DataContext with
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
        this.Styles.Add(FluentTheme(Uri("avares://Avalonia"), Mode = FluentThemeMode.Dark))
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